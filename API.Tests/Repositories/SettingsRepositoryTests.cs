using API.DataAccess;
using API.DTOs;
using API.Repositories;

namespace API.Tests.Repositories;

public class SettingsRepositoryTests
{
    private readonly Mock<ISqlDbAccess> _mockSqlDbAccess;
    private readonly SettingsRepository _settingsRepository;

    public SettingsRepositoryTests()
    {
        _mockSqlDbAccess = new Mock<ISqlDbAccess>();
        _settingsRepository = new SettingsRepository(_mockSqlDbAccess.Object);
    }

    [Fact]
    public void AddSetting_ShouldReturnNewSettingId()
    {
        // Arrange
        var setting = new SettingsDto(0, "TestSetting", "TestValue");
        var expectedId = 1;

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<int>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => 
                p.ContainsKey("@settingName") && 
                p.ContainsKey("@settingValue"))))
            .Returns(new List<int> { expectedId });

        // Act
        var result = _settingsRepository.AddSetting(setting);

        // Assert
        Assert.Equal(expectedId, result);
        _mockSqlDbAccess.Verify(db => db.ExecuteQuery<int>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => 
                p["@settingName"].Equals(setting.Name) && 
                p["@settingValue"].Equals(setting.Value))),
            Times.Once);
    }

    [Fact]
    public void GetSettingById_ShouldReturnSetting_WhenSettingExists()
    {
        // Arrange
        var settingId = 1;
        var expectedSetting = new SettingsDto(settingId, "TestSetting", "TestValue");

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<SettingsDto>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => p.ContainsKey("@settingId"))))
            .Returns(new List<SettingsDto> { expectedSetting });

        // Act
        var result = _settingsRepository.GetSettingById(settingId);

        // Assert
        Assert.Equal(expectedSetting.Id, result.Id);
        Assert.Equal(expectedSetting.Name, result.Name);
        Assert.Equal(expectedSetting.Value, result.Value);
    }

    [Fact]
    public void GetSettingById_ShouldThrowException_WhenSettingDoesNotExist()
    {
        // Arrange
        var settingId = 999;
        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<SettingsDto>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => p.ContainsKey("@settingId"))))
            .Returns(new List<SettingsDto>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _settingsRepository.GetSettingById(settingId));
        Assert.Contains($"Setting with ID {settingId} not found", exception.Message);
    }

    [Fact]
    public void GetAllSettings_ShouldReturnAllSettings()
    {
        // Arrange
        var expectedSettings = new List<SettingsDto>
        {
            new(1, "Setting1", "Value1"),
            new(2, "Setting2", "Value2"),
            new(3, "Setting3", "Value3")
        };

        _mockSqlDbAccess.Setup(db => db.ExecuteQuery<SettingsDto>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, object>>()))
            .Returns(expectedSettings);

        // Act
        var result = _settingsRepository.GetAllSettings();

        // Assert
        Assert.Equal(expectedSettings.Count, result.Count);
        for (int i = 0; i < expectedSettings.Count; i++)
        {
            Assert.Equal(expectedSettings[i].Id, result[i].Id);
            Assert.Equal(expectedSettings[i].Name, result[i].Name);
            Assert.Equal(expectedSettings[i].Value, result[i].Value);
        }
    }

    [Fact]
    public void UpdateSetting_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var setting = new SettingsDto(1, "UpdatedSetting", "UpdatedValue");

        // Act
        _settingsRepository.UpdateSetting(setting);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p =>
                p["@settingId"].Equals(setting.Id) &&
                p["@settingName"].Equals(setting.Name) &&
                p["@settingValue"].Equals(setting.Value))),
            Times.Once);
    }

    [Fact]
    public void DeleteSetting_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var settingId = 1;

        // Act
        _settingsRepository.DeleteSetting(settingId);

        // Assert
        _mockSqlDbAccess.Verify(db => db.ExecuteNonQuery(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<Dictionary<string, object>>(p => p["@settingId"].Equals(settingId))),
            Times.Once);
    }
}