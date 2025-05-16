using System.Collections.Concurrent;
using System.Diagnostics;

namespace API.Services;

public interface IInMemoryTokenService : IServiceBase
{
    void StoreRefreshToken(int userId, string refreshToken, DateTime expiryTime);
    RefreshTokenInfo GetRefreshToken(int userId);
    bool ValidateRefreshToken(int userId, string refreshToken);
    void RemoveRefreshToken(int userId);
    void CleanupExpiredTokens();
}
public class InMemoryTokenService : IInMemoryTokenService
{
    // Thread-safe dictionary to store refresh tokens
    private static readonly ConcurrentDictionary<int, RefreshTokenInfo> RefreshTokens = 
        new();

    public void StoreRefreshToken(int userId, string refreshToken, DateTime expiryTime)
    {
        Console.WriteLine("StoreRefreshToken valid until: " + expiryTime);
        RefreshTokens[userId] = new RefreshTokenInfo
        {
            Token = refreshToken,
            ExpiryTime = expiryTime
        };
    }

    public RefreshTokenInfo GetRefreshToken(int userId)
    {
        RefreshTokens.TryGetValue(userId, out var tokenInfo);
        return tokenInfo;
    }

    public bool ValidateRefreshToken(int userId, string refreshToken)
    {
        if (RefreshTokens.TryGetValue(userId, out var tokenInfo))
        {
            return tokenInfo.Token == refreshToken && tokenInfo.ExpiryTime > DateTime.UtcNow;
        }
        return false;
    }

    public void RemoveRefreshToken(int userId)
    {
        RefreshTokens.TryRemove(userId, out _);
    }

    public void CleanupExpiredTokens()
    {
        foreach (var userId in RefreshTokens.Keys)
        {
            if (RefreshTokens.TryGetValue(userId, out var tokenInfo) && tokenInfo.ExpiryTime <= DateTime.UtcNow)
            {
                RefreshTokens.TryRemove(userId, out _);
            }
        }
    }
}

public class RefreshTokenInfo
{
    public string Token { get; set; }
    public DateTime ExpiryTime { get; set; }
}