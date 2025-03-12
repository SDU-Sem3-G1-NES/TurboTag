using API.DTOs;

namespace API.Services;
public interface IUserCredentialsService : IServiceBase
{
    bool CheckIfUserExistsByEmail();
    void StoreNewUserCredentials();
    bool ValidateUserCredentials();
    UserDTO GetUserDataByEmail();
    void StoreUserSession();
    UserDTO GetUserDataBySession();
}