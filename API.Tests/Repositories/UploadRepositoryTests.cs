using API.DataAccess;
using API.DTOs;
using API.Repositories;

namespace API.Tests.Repositories;

public class UploadRepositoryTests
{
    private readonly Mock<ISqlDbAccess> _mockSqlDbAccess;
    private readonly UploadRepository _uploadRepository;

    public UploadRepositoryTests()
    {
        _mockSqlDbAccess = new Mock<ISqlDbAccess>();
        _uploadRepository = new UploadRepository(_mockSqlDbAccess.Object);
    }

    [Fact]
    public void AddUpload_ShouldReturnNewUploadId()
    {
        // Arrange
        var upload = new UploadDto(0, 1, DateTime.Now, "image/png", 1);
        var expectedId = 1;

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p.ContainsKey("@userId") &&
                    p.ContainsKey("@uploadDate") &&
                    p.ContainsKey("@uploadType"))))
            .Returns(new List<int> { expectedId });

        // Act
        var result = _uploadRepository.AddUpload(upload);

        // Assert
        Assert.Equal(expectedId, result);

        // Verify upload insertion
        _mockSqlDbAccess.Verify(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@userId"].Equals(upload.OwnerId) &&
                    p["@uploadDate"].Equals(upload.Date) &&
                    p["@uploadType"].Equals(upload.Type))),
            Times.Once);

        // Verify library association
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@libraryId"].Equals(upload.LibraryId) &&
                    p["@uploadId"].Equals(expectedId))),
            Times.Once);
    }

    [Fact]
    public void GetUploadById_ShouldReturnUpload_WhenUploadExists()
    {
        // Arrange
        var uploadId = 1;
        var expectedUpload = new UploadDto(uploadId, 1, DateTime.Now, "image/png", 1);

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UploadDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p.ContainsKey("@uploadId"))))
            .Returns(new List<UploadDto> { expectedUpload });

        // Act
        var result = _uploadRepository.GetUploadById(uploadId);

        // Assert
        Assert.Equal(expectedUpload.Id, result.Id);
        Assert.Equal(expectedUpload.OwnerId, result.OwnerId);
        Assert.Equal(expectedUpload.Type, result.Type);
        Assert.Equal(expectedUpload.LibraryId, result.LibraryId);
    }

    [Fact]
    public void GetUploadById_ShouldThrowException_WhenUploadDoesNotExist()
    {
        // Arrange
        var uploadId = 999;
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UploadDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p.ContainsKey("@uploadId"))))
            .Returns(new List<UploadDto>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _uploadRepository.GetUploadById(uploadId));
        Assert.Contains($"Upload with ID {uploadId} not found", exception.Message);
    }

    [Fact]
    public void GetAllUploads_ShouldReturnAllUploads_WhenNoFilterProvided()
    {
        // Arrange
        var expectedUploads = new List<UploadDto>
        {
            new(1, 1, DateTime.Now, "image/png", 1),
            new(2, 2, DateTime.Now, "image/jpeg", 1),
            new(3, 1, DateTime.Now, "application/pdf", 2)
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UploadDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(expectedUploads);

        // Act
        var result = _uploadRepository.GetAllUploads();

        // Assert
        Assert.Equal(expectedUploads.Count, result.Items.Count);
        Assert.Equal(expectedUploads.Count, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void GetAllUploads_ShouldApplyFilters_WhenFilterProvided()
    {
        // Arrange
        var filter = new UploadFilter(
            new List<int> { 1 },
            new List<int> { 1 },
            new List<int> { 1 },
            new List<string> { "image/png" },
            DateTime.Now.AddDays(-1),
            DateTime.Now,
            1,
            10
        );

        var expectedPagedUploads = new PagedResult<UploadDto>
        {
            new(1, 1, DateTime.Now, "image/png", 1)
        };

        var expectedUploads = new List<UploadDto>
        {
            new(1, 1, DateTime.Now, "image/png", 1)
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(new List<int> { expectedUploads.Count });

        _mockSqlDbAccess.Setup(db => db.GetPagedResult<UploadDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(expectedPagedUploads);

        // Act
        var result = _uploadRepository.GetAllUploads(filter);

        // Assert
        Assert.Equal(expectedUploads.Count, result.Items.Count);
        Assert.Equal(expectedPagedUploads.Count(), result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void UpdateUpload_ShouldUpdateUploadAndLibraryAssociation()
    {
        // Arrange
        var upload = new UploadDto(1, 2, DateTime.Now, "updated/type", 3);

        // Act
        _uploadRepository.UpdateUpload(upload);

        // Assert
        // Verify upload update
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p.ContainsKey("@uploadId") &&
                    p["@uploadId"].Equals(upload.Id) &&
                    p.ContainsKey("@userId") &&
                    p["@userId"].Equals(upload.OwnerId) &&
                    p.ContainsKey("@uploadDate") &&
                    p["@uploadDate"].Equals(upload.Date) &&
                    p.ContainsKey("@uploadType") &&
                    p["@uploadType"].Equals(upload.Type))),
            Times.Once);

        // Verify library association deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM library_uploads")),
                It.Is<Dictionary<string, object>>(p => p["@uploadId"].Equals(upload.Id))),
            Times.Once);

        // Verify library association creation
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("INSERT INTO library_uploads")),
                It.Is<Dictionary<string, object>>(p =>
                    p["@libraryId"].Equals(upload.LibraryId) &&
                    p["@uploadId"].Equals(upload.Id))),
            Times.Once);
    }

    [Fact]
    public void DeleteUploadById_ShouldDeleteUploadAndAssociations()
    {
        // Arrange
        var uploadId = 1;

        // Act
        _uploadRepository.DeleteUploadById(uploadId);

        // Assert
        // Verify library association deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM library_uploads")),
                It.Is<Dictionary<string, object>>(p => p["@uploadId"].Equals(uploadId))),
            Times.Once);

        // Verify upload deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM uploads")),
                It.Is<Dictionary<string, object>>(p => p["@uploadId"].Equals(uploadId))),
            Times.Once);
    }
}