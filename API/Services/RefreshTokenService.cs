using API.Repositories;

namespace API.Services;

public interface IRefreshTokenService : IServiceBase
{
    void Store(int userId, string refreshToken, DateTime expiryTime);
    bool Validate(int userId, string refreshToken);
    void Remove(int userId);
    void CleanupExpired();
}
public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public void Store(int userId, string refreshToken, DateTime expiryTime)
    {
        refreshTokenRepository.RemoveRefreshToken(userId);
        refreshTokenRepository.StoreRefreshToken(userId, refreshToken, expiryTime);
    }

    public bool Validate(int userId, string refreshToken)
    {
        return refreshTokenRepository.GetRefreshTokenExpiry(userId, refreshToken) > DateTime.UtcNow;
    }

    public void Remove(int userId)
    {
        refreshTokenRepository.RemoveRefreshToken(userId);
    }

    public void CleanupExpired()
    {
        refreshTokenRepository.CleanupExpiredTokens();
    }
}