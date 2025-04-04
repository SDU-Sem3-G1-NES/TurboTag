using System.Text.Json;
using API.DataAccess;
using API.DTOs;
using API.Repositories;

namespace API.Tests.Repositories;

public class UserTypeRepositoryTests
{
    private readonly Mock<ISqlDbAccess> _mockSqlDbAccess;
    private readonly UserTypeRepository _userTypeRepository;

    public UserTypeRepositoryTests()
    {
        _mockSqlDbAccess = new Mock<ISqlDbAccess>();
        _userTypeRepository = new UserTypeRepository(_mockSqlDbAccess.Object);
    }
    
    private class UserTypeDatabase
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Permissions { get; set; }
    }

    [Fact]
    public void AddUserType_ShouldReturnNewUserTypeId()
    {
        // Arrange
        var permissions = new Dictionary<string, bool> { { "read", true }, { "write", false } };
        var userType = new UserTypeDto(0, "Admin", permissions);
        var expectedId = 1;

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, object>>()))
            .Returns(new List<int> { expectedId });

        // Act
        var result = _userTypeRepository.AddUserType(userType);

        // Assert
        Assert.Equal(expectedId, result);

        // Verify the query execution
        _mockSqlDbAccess.Verify(db => db.ExecuteQuery<int>(
            It.IsAny<string>(),
            It.Is<string>(sql => sql.Contains("INSERT INTO user_types")),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => 
                p.ContainsKey("@typeName") && p["@typeName"].Equals(userType.Name) &&
                p.ContainsKey("@typePermissions"))),
            Times.Once);
    }

    [Fact]
    public void GetUserTypeById_ShouldReturnUserType_WhenUserTypeExists()
    {
        // Arrange
        var userTypeId = 1;
        var userTypeName = "Admin";
        var permissions = new Dictionary<string, bool> { { "read", true }, { "write", false } };
        var serializedPermissions = JsonSerializer.Serialize(permissions);

        var dbResult = new UserTypeRepository.UserTypeDatabase
        {
            Id = userTypeId,
            Name = userTypeName,
            Permissions = serializedPermissions
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserTypeRepository.UserTypeDatabase>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => p.ContainsKey("@typeId") && p["@typeId"].Equals(userTypeId))))
            .Returns(new List<UserTypeRepository.UserTypeDatabase> { dbResult }.AsEnumerable());

        // Act
        var result = _userTypeRepository.GetUserTypeById(userTypeId);

        // Assert
        Assert.Equal(userTypeId, result.Id);
        Assert.Equal(userTypeName, result.Name);
        Assert.Equal(permissions.Count, result.Permissions.Count);
        Assert.True(result.Permissions["read"]);
        Assert.False(result.Permissions["write"]);
    }

    [Fact]
    public void GetAllUserTypes_ShouldReturnAllUserTypes()
    {
        // Arrange
        var userTypesDb = new List<UserTypeRepository.UserTypeDatabase>
        {
            new UserTypeRepository.UserTypeDatabase
            {
                Id = 1,
                Name = "Admin",
                Permissions = JsonSerializer.Serialize(new Dictionary<string, bool> { { "fullAccess", true } })
            },
            new UserTypeRepository.UserTypeDatabase
            {
                Id = 2,
                Name = "User",
                Permissions = JsonSerializer.Serialize(new Dictionary<string, bool> { { "read", true }, { "write", false } })
            }
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<UserTypeRepository.UserTypeDatabase>(
            It.IsAny<string>(),
            It.Is<string>(sql => sql.Contains("SELECT")),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, object>>()))
            .Returns(userTypesDb);

        // Act
        var result = _userTypeRepository.GetAllUserTypes();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Admin", result[0].Name);
        Assert.True(result[0].Permissions["fullAccess"]);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("User", result[1].Name);
        Assert.True(result[1].Permissions["read"]);
        Assert.False(result[1].Permissions["write"]);
    }

    [Fact]
    public void UpdateUserType_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var permissions = new Dictionary<string, bool> { { "read", true }, { "write", true } };
        var userType = new UserTypeDto(1, "Updated Admin", permissions);

        // Act
        _userTypeRepository.UpdateUserType(userType);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
            It.IsAny<string>(),
            It.Is<string>(sql => sql.Contains("UPDATE user_types")),
            It.Is<Dictionary<string, object>>(p => 
                p.ContainsKey("@typeId") && p["@typeId"].Equals(userType.Id) &&
                p.ContainsKey("@typeName") && p["@typeName"].Equals(userType.Name) &&
                p.ContainsKey("@typePermissions"))),
            Times.Once);
    }

    [Fact]
    public void DeleteUserType_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var userTypeId = 1;

        // Act
        _userTypeRepository.DeleteUserTypeById(userTypeId);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
            It.IsAny<string>(),
            It.Is<string>(sql => sql.Contains("DELETE FROM user_types")),
            It.Is<Dictionary<string, object>>(p => p.ContainsKey("@typeId") && p["@typeId"].Equals(userTypeId))),
            Times.Once);
    }
}