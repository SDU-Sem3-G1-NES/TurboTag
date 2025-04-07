using API.DataAccess;
using API.DTOs;
using API.Repositories;

namespace API.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ISqlDbAccess> _mockSqlDbAccess;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _mockSqlDbAccess = new Mock<ISqlDbAccess>();
        _userRepository = new UserRepository(_mockSqlDbAccess.Object);
    }

    [Fact]
    public void AddUser_ShouldReturnNewUserId()
    {
        // Arrange
        var user = new UserDto(0, 1, "Test User", "test@example.com", new List<int> { 1, 2 });
        var expectedId = 1;

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p.ContainsKey("@userTypeId") &&
                    p.ContainsKey("@userName") &&
                    p.ContainsKey("@userEmail"))))
            .Returns(new List<int> { expectedId });

        // Act
        var result = _userRepository.AddUser(user);

        // Assert
        Assert.Equal(expectedId, result);

        // Verify user insertion
        _mockSqlDbAccess.Verify(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@userTypeId"].Equals(user.UserTypeId) &&
                    p["@userName"].Equals(user.Name) &&
                    p["@userEmail"].Equals(user.Email))),
            Times.Once);

        // Verify library access entries
        foreach (var libraryId in user.AccessibleLibraryIds)
            _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(p =>
                        p["@userId"].Equals(expectedId) &&
                        p["@libraryId"].Equals(libraryId))),
                Times.Once);
    }

    [Fact]
    public void AddUserCredentials_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var hashedUserCredentials = new HashedUserCredentialsDto(1, new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 });

        // Act
        _userRepository.AddUserCredentials(hashedUserCredentials);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@userId"].Equals(hashedUserCredentials.UserId) &&
                    p["@passwordHash"].Equals(hashedUserCredentials.PasswordHash) &&
                    p["@passwordSalt"].Equals(hashedUserCredentials.PasswordSalt))),
            Times.Once);
    }

    [Fact]
    public void UserEmailExists_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var email = "test@example.com";
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@email"].Equals(email))))
            .Returns(new List<int> { 1 });

        // Act
        var result = _userRepository.UserEmailExists(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserEmailExists_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@email"].Equals(email))))
            .Returns(new List<int> { 0 });

        // Act
        var result = _userRepository.UserEmailExists(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new UserDto(userId, 1, "Test User", "test@example.com", new List<int>());
        var libraryIds = new List<int> { 1, 2 };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("user_id = @userId")),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))))
            .Returns(new List<UserDto> { expectedUser });

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("library_id")),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))))
            .Returns(libraryIds);

        // Act
        var result = _userRepository.GetUserById(userId);

        // Assert
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.UserTypeId, result.UserTypeId);
        Assert.Equal(expectedUser.Name, result.Name);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(libraryIds, result.AccessibleLibraryIds);
    }

    [Fact]
    public void GetUserById_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("user_id = @userId")),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))))
            .Returns(new List<UserDto>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _userRepository.GetUserById(userId));
        Assert.Contains($"User with email {userId} not found", exception.Message);
    }

    [Fact]
    public void GetUserByEmail_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new UserDto(1, 1, "Test User", email, new List<int>());
        var libraryIds = new List<int> { 1, 2 };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("user_email = @email")),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@email"].Equals(email))))
            .Returns(new List<UserDto> { expectedUser });

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("library_id")),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(expectedUser.Id))))
            .Returns(libraryIds);

        // Act
        var result = _userRepository.GetUserByEmail(email);

        // Assert
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.UserTypeId, result.UserTypeId);
        Assert.Equal(expectedUser.Name, result.Name);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(libraryIds, result.AccessibleLibraryIds);
    }

    [Fact]
    public void GetUserByEmail_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("user_email = @email")),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@email"].Equals(email))))
            .Returns(new List<UserDto>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _userRepository.GetUserByEmail(email));
        Assert.Contains($"User with email {email} not found", exception.Message);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnAllUsers_WhenNoFilterProvided()
    {
        // Arrange
        var expectedUsers = new List<UserDto>
        {
            new(1, 1, "User 1", "user1@example.com", new List<int> { 1 }),
            new(2, 2, "User 2", "user2@example.com", new List<int> { 2 })
        };

        // Important: Set up the library IDs inside the user objects
        // since array_agg returns them directly in the main query
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(expectedUsers);

        // Act
        var result = _userRepository.GetAllUsers().ToList();

        // Assert
        Assert.Equal(expectedUsers.Count, result.Count);
        for (var i = 0; i < expectedUsers.Count; i++)
        {
            Assert.Equal(expectedUsers[i].Id, result[i].Id);
            Assert.Equal(expectedUsers[i].UserTypeId, result[i].UserTypeId);
            Assert.Equal(expectedUsers[i].Name, result[i].Name);
            Assert.Equal(expectedUsers[i].Email, result[i].Email);
            Assert.Contains(expectedUsers[i].Id, result[i].AccessibleLibraryIds);
        }
    }

    [Fact]
    public void GetAllUsers_ShouldApplyFilters_WhenFilterProvided()
    {
        // Arrange
        var filter = new UserFilter(
            new List<int> { 1 },
            new List<int> { 1 },
            "Test",
            "test",
            1,
            1,
            10
        );

        // Include the library ID in the expected user
        var expectedUser = new UserDto(1, 1, "Test User", "test@example.com", new List<int> { 1 });
        var expectedUsers = new List<UserDto> { expectedUser };

        var pagedResult = new PagedResult<UserDto>
        {
            Items = expectedUsers,
            TotalCount = expectedUsers.Count,
            PageSize = 10,
            CurrentPage = 1,
            TotalPages = 1
        };

        // Return users with library IDs already populated
        _mockSqlDbAccess.Setup(db => db.GetPagedResult<UserDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.Is<int>(p => p == filter.PageNumber.Value),
                It.Is<int>(p => p == filter.PageSize.Value)))
            .Returns(pagedResult);

        // Act
        var result = _userRepository.GetAllUsers(filter);

        // Assert
        Assert.Equal(expectedUsers.Count, result.Count());
        Assert.Equal(expectedUser.Id, result.First().Id);
        Assert.Equal(expectedUser.UserTypeId, result.First().UserTypeId);
        Assert.Equal(expectedUser.Name, result.First().Name);
        Assert.Equal(expectedUser.Email, result.First().Email);
        Assert.Contains(1, result.First().AccessibleLibraryIds);
    }

    [Fact]
    public void GetUserCredentials_ShouldReturnCredentials()
    {
        // Arrange
        var userId = 1;
        var expectedHash = new byte[] { 1, 2, 3 };
        var expectedSalt = new byte[] { 4, 5, 6 };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<HashedUserCredentialsDto>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))))
            .Returns(new List<HashedUserCredentialsDto>
            {
                new(userId, expectedHash, expectedSalt)
            });

        // Act
        var result = _userRepository.GetUserCredentials(userId);

        // Assert
        Assert.Equal(userId, result.UserId);
        Assert.Equal(expectedHash, result.PasswordHash);
        Assert.Equal(expectedSalt, result.PasswordSalt);
    }

    [Fact]
    public void UpdateUser_ShouldUpdateUserAndLibraryAccess()
    {
        // Arrange
        var user = new UserDto(1, 2, "Updated User", "updated@example.com", new List<int> { 3, 4 });

        // Act
        _userRepository.UpdateUser(user);

        // Assert
        // Verify user update
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p.ContainsKey("@userId") &&
                    p["@userId"].Equals(user.Id) &&
                    p.ContainsKey("@userTypeId") &&
                    p["@userTypeId"].Equals(user.UserTypeId) &&
                    p.ContainsKey("@userName") &&
                    p["@userName"].Equals(user.Name) &&
                    p.ContainsKey("@userEmail") &&
                    p["@userEmail"].Equals(user.Email))),
            Times.Once);

        // Verify library access deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM user_library_access")),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(user.Id))),
            Times.Once);

        // Verify library access creation
        foreach (var libraryId in user.AccessibleLibraryIds)
            _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.Is<string>(sql => sql.Contains("INSERT INTO user_library_access")),
                    It.Is<Dictionary<string, object>>(p =>
                        p["@userId"].Equals(user.Id) &&
                        p["@libraryId"].Equals(libraryId))),
                Times.Once);
    }

    [Fact]
    public void UpdateUserCredentials_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var hashedUserCredentials = new HashedUserCredentialsDto(1, new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 });

        // Act
        _userRepository.UpdateUserCredentials(hashedUserCredentials);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<Dictionary<string, object>>(p =>
                    p["@userId"].Equals(hashedUserCredentials.UserId) &&
                    p["@passwordHash"].Equals(hashedUserCredentials.PasswordHash) &&
                    p["@passwordSalt"].Equals(hashedUserCredentials.PasswordSalt))),
            Times.Once);
    }

    [Fact]
    public void DeleteUser_ShouldDeleteUserAndRelatedRecords()
    {
        // Arrange
        var userId = 1;

        // Act
        _userRepository.DeleteUserById(userId);

        // Assert
        // Verify credentials deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM user_credentials")),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))),
            Times.Once);

        // Verify library access deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM user_library_access")),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))),
            Times.Once);

        // Verify user deletion
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<string>(sql => sql.Contains("DELETE FROM users")),
                It.Is<Dictionary<string, object>>(p => p["@userId"].Equals(userId))),
            Times.Once);
    }
}