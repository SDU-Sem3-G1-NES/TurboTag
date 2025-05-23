using API.DataAccess;
using API.DTOs;

namespace API.Repositories;

public interface IUserRepository : IRepositoryBase
{
    int AddUser(UserDto user);
    void AddUserCredentials(HashedUserCredentialsDto userCredentials);
    UserDto GetUserById(int userId);
    UserDto? GetUserByEmail(string email);
    public IEnumerable<UserDto> GetAllUsers(UserFilter? filter = null);
    HashedUserCredentialsDto GetUserCredentials(int userId);
    void UpdateUser(UserDto user);
    void UpdateUserCredentials(HashedUserCredentialsDto userCredentials);
    void DeleteUserById(int userId);
}

public class UserRepository(ISqlDbAccess sqlDbAccess) : IUserRepository
{
    private readonly string _databaseName = "blank";

    public int AddUser(UserDto user)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userTypeId", user.UserTypeId },
            { "@userName", user.Name },
            { "@userEmail", user.Email }
        };

        var insertSql = @"
            INSERT INTO users (user_type_id, user_name, user_email)
            VALUES (@userTypeId, @userName, @userEmail)
            RETURNING user_id;";

        var userId = sqlDbAccess.ExecuteQuery<int>(
            _databaseName,
            insertSql,
            "",
            "",
            parameters).FirstOrDefault();

        return userId;
    }

    public void AddUserCredentials(HashedUserCredentialsDto userCredentials)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userCredentials.UserId },
            { "@passwordHash", userCredentials.PasswordHash },
            { "@passwordSalt", userCredentials.PasswordSalt }
        };

        var insertSql = @"
            INSERT INTO user_credentials (user_id, user_password_hash, user_password_salt)
            VALUES (@userId, @passwordHash, @passwordSalt);";

        sqlDbAccess.ExecuteNonQuery(_databaseName, insertSql, parameters);
    }

    public UserDto GetUserById(int userId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userId }
        };

        var selectSql = @"
            SELECT
                u.user_id as Id,
                u.user_type_id as UserTypeId,
                u.user_name as Name,
                u.user_email as Email
            ";

        var fromWhereSql = @"
            FROM users u
            WHERE u.user_id = @userId";

        var user = sqlDbAccess.ExecuteQuery<UserDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            "",
            parameters).FirstOrDefault();

        if (user == null) throw new InvalidOperationException($"User with email {userId} not found");

        return user;
    }

    public UserDto? GetUserByEmail(string email)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@email", email }
        };

        var selectSql = @"
            SELECT
                u.user_id as Id,
                u.user_type_id as UserTypeId,
                u.user_name as Name,
                u.user_email as Email
    ";

        var fromWhereSql = @"
            FROM users u
            WHERE u.user_email = @email";

        var user = sqlDbAccess.ExecuteQuery<UserDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            "",
            parameters).FirstOrDefault();

        return user;
    }

    public IEnumerable<UserDto> GetAllUsers(UserFilter? filter = null)
    {
        var selectSql = @"
            SELECT
                u.user_id as Id,
                u.user_type_id as UserTypeId,
                u.user_name as Name,
                u.user_email as Email,
                COALESCE(array_remove(array_agg(ula.library_id), NULL), '{}'::integer[]) AS AccessibleLibraryIds ";

        var fromWhereSql = @"FROM users u 
                                LEFT JOIN user_library_access ula ON ula.user_id = u.user_id
                                WHERE 1=1";

        var parameters = new Dictionary<string, object>();

        if (filter != null)
        {
            if (filter.UserIds != null && filter.UserIds.Any())
            {
                parameters.Add("@userIds", filter.UserIds);
                fromWhereSql += " AND u.user_id = ANY(@userIds)";
            }

            if (filter.UserTypeIds != null && filter.UserTypeIds.Any())
            {
                parameters.Add("@userTypeIds", filter.UserTypeIds);
                fromWhereSql += " AND u.user_type_id = ANY(@userTypeIds)";
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                parameters.Add("@name", $"%{filter.Name}%");
                fromWhereSql += " AND u.user_name ILIKE @name";
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                parameters.Add("@email", $"%{filter.Email}%");
                fromWhereSql += " AND u.user_email LIKE @email";
            }
        }

        var orderBy = @" GROUP BY u.user_id, u.user_type_id, u.user_name, u.user_email 
        ORDER BY u.user_id";

        if (filter is { PageSize: not null, PageNumber: not null })
            return sqlDbAccess.GetPagedResult<UserDto>(
                _databaseName,
                selectSql,
                fromWhereSql,
                orderBy,
                parameters,
                filter.PageNumber.Value,
                filter.PageSize.Value);
        return sqlDbAccess.ExecuteQuery<UserDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            orderBy,
            parameters).ToList();
    }

    public HashedUserCredentialsDto GetUserCredentials(int userId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userId }
        };

        var sql = @"SELECT 
            user_password_hash as PasswordHash, 
            user_password_salt as PasswordSalt 
            FROM user_credentials 
            WHERE user_id = @userId";

        var result = sqlDbAccess.ExecuteQuery<HashedUserCredentialsDto>(
            _databaseName,
            sql,
            "",
            "",
            parameters).FirstOrDefault();

        if (result == null) throw new InvalidOperationException($"User credentials for user ID {userId} not found");

        return new HashedUserCredentialsDto(result.UserId, result.PasswordHash, result.PasswordSalt);
    }

    public void UpdateUser(UserDto user)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", user.Id },
            { "@userTypeId", user.UserTypeId },
            { "@userName", user.Name },
            { "@userEmail", user.Email }
        };

        var updateSql = @"
            UPDATE users
            SET user_type_id = @userTypeId,
                user_name = @userName,
                user_email = @userEmail
            WHERE user_id = @userId";

        sqlDbAccess.ExecuteNonQuery(_databaseName, updateSql, parameters);
    }

    public void UpdateUserCredentials(HashedUserCredentialsDto userCredentials)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userCredentials.UserId },
            { "@passwordHash", userCredentials.PasswordHash },
            { "@passwordSalt", userCredentials.PasswordSalt }
        };

        var updateSql = @"
            UPDATE user_credentials
            SET user_password_hash = @passwordHash,
                user_password_salt = @passwordSalt
            WHERE user_id = @userId";

        sqlDbAccess.ExecuteNonQuery(_databaseName, updateSql, parameters);
    }

    public void DeleteUserById(int userId)
    {
        // Delete user credentials
        var deleteCredentialsSql = @"DELETE FROM user_credentials WHERE user_id = @userId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteCredentialsSql,
            new Dictionary<string, object> { { "@userId", userId } });

        // Delete library access entries
        var deleteLibraryAccessSql = @"DELETE FROM user_library_access WHERE user_id = @userId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteLibraryAccessSql,
            new Dictionary<string, object> { { "@userId", userId } });

        // Delete the user
        var deleteUserSql = @"DELETE FROM users WHERE user_id = @userId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteUserSql,
            new Dictionary<string, object> { { "@userId", userId } });
    }
}

public class UserFilter : PaginationFilter
{
    public UserFilter(List<int>? userIds,
        List<int>? userTypeIds,
        string? name,
        string? email,
        int? libraryId,
        int? pageNumber,
        int? pageSize) : base(pageNumber, pageSize)
    {
        UserIds = userIds;
        UserTypeIds = userTypeIds;
        Name = name;
        Email = email;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public UserFilter()
    {
    }

    public List<int>? UserIds { get; set; }
    public List<int>? UserTypeIds { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}