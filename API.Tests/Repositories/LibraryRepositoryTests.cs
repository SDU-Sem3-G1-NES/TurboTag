using API.DataAccess;
using API.DTOs;
using API.Repositories;

namespace API.Tests.Repositories;

public class LibraryRepositoryTests
{
    private readonly LibraryRepository _libraryRepository;
    private readonly Mock<ISqlDbAccess> _mockSqlDbAccess;

    public LibraryRepositoryTests()
    {
        _mockSqlDbAccess = new Mock<ISqlDbAccess>();
        _libraryRepository = new LibraryRepository(_mockSqlDbAccess.Object);
    }

    [Fact]
    public void AddLibrary_ShouldReturnNewLibraryId()
    {
        // Arrange
        var library = new LibraryDto(0, "Test Library");
        var expectedId = 1;

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p.ContainsKey("@libraryName"))))
            .Returns(new List<int> { expectedId });

        // Act
        var result = _libraryRepository.AddLibrary(library);

        // Assert
        Assert.Equal(expectedId, result);
        _mockSqlDbAccess.Verify(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@libraryName"].Equals(library.Name))),
            Times.Once);
    }

    [Fact]
    public void GetLibraryById_ShouldReturnLibrary_WhenLibraryExists()
    {
        // Arrange
        var libraryId = 1;
        var expectedLibrary = new LibraryDto(libraryId, "Test Library");

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<LibraryDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p.ContainsKey("@libraryId"))))
            .Returns(new List<LibraryDto> { expectedLibrary });

        // Act
        var result = _libraryRepository.GetLibraryById(libraryId);

        // Assert
        Assert.Equal(expectedLibrary.Id, result.Id);
        Assert.Equal(expectedLibrary.Name, result.Name);
    }

    [Fact]
    public void GetLibraryById_ShouldThrowException_WhenLibraryDoesNotExist()
    {
        // Arrange
        var libraryId = 999;
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<LibraryDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p.ContainsKey("@libraryId"))))
            .Returns(new List<LibraryDto>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _libraryRepository.GetLibraryById(libraryId));
        Assert.Contains($"Library with ID {libraryId} not found", exception.Message);
    }

    [Fact]
    public void GetAllLibraries_ShouldReturnAllLibraries_WhenNoFilterProvided()
    {
        // Arrange
        var expectedLibraries = new List<LibraryDto>
        {
            new(1, "Library 1"),
            new(2, "Library 2"),
            new(3, "Library 3")
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<LibraryDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(expectedLibraries);

        // Act
        var result = _libraryRepository.GetAllLibraries();

        // Assert
        Assert.Equal(expectedLibraries.Count, result.Items.Count);
        Assert.Equal(expectedLibraries.Count, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void GetAllLibraries_ShouldApplyFilters_WhenFilterProvided()
    {
        // Arrange
        var filter = new LibraryFilter(
            new List<int> { 1, 2 },
            new List<string> { "Library 1" },
            1,
            10
        );

        var expectedPagedLibraries = new PagedResult<LibraryDto>
        {
            new(1, "Library 1")
        };

        var expectedLibraries = new List<LibraryDto>
        {
            new(1, "Library 1")
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(new List<int> { expectedLibraries.Count });

        _mockSqlDbAccess.Setup(db => db.GetPagedResult<LibraryDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(expectedPagedLibraries);

        // Act
        var result = _libraryRepository.GetAllLibraries(filter);

        // Assert
        Assert.Equal(expectedLibraries.Count, result.Items.Count);
        Assert.Equal(expectedPagedLibraries.Count(), result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void UpdateLibrary_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var library = new LibraryDto(1, "Updated Library");

        // Act
        _libraryRepository.UpdateLibrary(library);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@libraryId"].Equals(library.Id) &&
                    p["@libraryName"].Equals(library.Name))),
            Times.Once);
    }

    [Fact]
    public void DeleteLibrary_ShouldDeleteLibraryAndRelatedRecords()
    {
        // Arrange
        var libraryId = 1;

        // Act
        _libraryRepository.DeleteLibraryById(libraryId);

        // Assert
        // Verify user access deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM user_library_access")),
                It.Is<Dictionary<string, object>>(p => p["@libraryId"].Equals(libraryId))),
            Times.Once);

        // Verify library uploads deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM library_uploads")),
                It.Is<Dictionary<string, object>>(p => p["@libraryId"].Equals(libraryId))),
            Times.Once);

        // Verify library deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM libraries")),
                It.Is<Dictionary<string, object>>(p => p["@libraryId"].Equals(libraryId))),
            Times.Once);
    }
}