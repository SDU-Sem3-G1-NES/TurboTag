using API.DataAccess;

namespace API.Repositories;

public interface IRefreshTokenRepository : IRepositoryBase
{
    void StoreRefreshToken(int userId, string refreshToken, DateTime expiryTime);
    DateTime GetRefreshTokenExpiry(int userId, string refreshToken);
    void RemoveRefreshToken(int userId);
    void CleanupExpiredTokens();
}
public class RefreshTokenRepository(ISqlDbAccess sqlDbAccess) : IRefreshTokenRepository
{
    private readonly string _databaseName = "blank";
    
    public void StoreRefreshToken(int userId, string refreshToken, DateTime expiryTime)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userId },
            { "@refreshToken", refreshToken },
            { "@expiryTime", expiryTime }
        };

        var insertSql = @"
            INSERT INTO refresh_tokens (user_id, token, expiry)
            VALUES (@userId, @refreshToken, @expiryTime)";

        sqlDbAccess.ExecuteNonQuery(_databaseName, insertSql, parameters);
    }

    public DateTime GetRefreshTokenExpiry(int userId, string refreshToken)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userId },
            { "@refreshToken", refreshToken }
        };
        var selectSql = @"
            SELECT expiry
            FROM refresh_tokens
            WHERE user_id = @userId AND token = @refreshToken";
        
        var result = sqlDbAccess.ExecuteQuery<DateTime>(
            _databaseName,
            selectSql,
            "",
            "",
            parameters).FirstOrDefault();
        
        if (result == DateTime.MinValue) throw new InvalidOperationException($"No expiry found for user {userId} and token {refreshToken}");
        
        return result;
    }

    public void RemoveRefreshToken(int userId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", userId }
        };

        var deleteSql = @"
            DELETE FROM refresh_tokens
            WHERE user_id = @userId";

        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteSql, parameters);
    }

    public void CleanupExpiredTokens()
    {
        var parameters = new Dictionary<string, object>
        {
            { "@currentTime", DateTime.UtcNow }
        };

        var deleteSql = @"
        DELETE FROM refresh_tokens
        WHERE expiry < @currentTime";

        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteSql, parameters);
    }
}