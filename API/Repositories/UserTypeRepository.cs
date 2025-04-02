using API.DTOs;
using API.DataAccess;
using System.Text.Json;

namespace API.Repositories;

public interface IUserTypeRepository : IRepositoryBase
{
    int AddUserType(UserTypeDto userType);
    UserTypeDto GetUserTypeById(int userTypeId);
    List<UserTypeDto> GetAllUserTypes();
    void UpdateUserType(UserTypeDto userType);
    void DeleteUserType(int userTypeId);
}
public class UserTypeRepository(ISqlDbAccess sqlDbAccess) : IUserTypeRepository
{
    private readonly string _databaseName = "blank";
    
    public int AddUserType(UserTypeDto userType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@typeName", userType.Name },
            { "@typePermissions", JsonSerializer.Serialize(userType.Permissions) }
        };

        var insertSql = @"
            INSERT INTO user_types (user_type_name, user_type_permissions)
            VALUES (@typeName, @typePermissions);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var userTypeId = sqlDbAccess.ExecuteQuery<int>(
            _databaseName,
            insertSql,
            "",
            "",
            parameters).FirstOrDefault();

        return userTypeId;
    }

    public UserTypeDto GetUserTypeById(int userTypeId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@typeId", userTypeId }
        };

        var selectSql = @"
            SELECT
                user_type_id as Id,
                user_type_name as Name,
                user_type_permissions as Permissions
            FROM user_types
            WHERE user_type_id = @typeId";

        var result = sqlDbAccess.ExecuteQuery<UserTypeDatabase>(
            _databaseName,
            selectSql,
            "",
            "",
            parameters).FirstOrDefault();

        if (result == null)
        {
            throw new InvalidOperationException($"User type with ID {userTypeId} not found");
        }
        
        return new UserTypeDto(
            result.Id, 
            result.Name, 
            JsonSerializer.Deserialize<Dictionary<string, bool>>(result.Permissions) ?? new Dictionary<string, bool>());
    }

    public List<UserTypeDto> GetAllUserTypes()
    {
        var selectSql = @"
            SELECT
                user_type_id as Id,
                user_type_name as Name,
                user_type_permissions as Permissions
            FROM user_types";

        var results = sqlDbAccess.ExecuteQuery<UserTypeDatabase>(
            _databaseName,
            selectSql,
            "",
            "",
            new Dictionary<string, object>()).ToList();

        return results.Select(r => new UserTypeDto(
            r.Id,
            r.Name,
            JsonSerializer.Deserialize<Dictionary<string, bool>>(r.Permissions) ?? new Dictionary<string, bool>()
        )).ToList();
    }

    public void UpdateUserType(UserTypeDto userType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@typeId", userType.Id },
            { "@typeName", userType.Name },
            { "@typePermissions", JsonSerializer.Serialize(userType.Permissions) }
        };

        var updateSql = @"
            UPDATE user_types
            SET user_type_name = @typeName,
                user_type_permissions = @typePermissions
            WHERE user_type_id = @typeId";

        sqlDbAccess.ExecuteNonQuery(_databaseName, updateSql, parameters);
    }

    public void DeleteUserType(int userTypeId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@typeId", userTypeId }
        };

        var deleteSql = @"DELETE FROM user_types WHERE user_type_id = @typeId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteSql, parameters);
    }
    
    public class UserTypeDatabase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }
    }
}