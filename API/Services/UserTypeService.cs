using API.DTOs;
using API.Repositories;

namespace API.Services;

public interface IUserTypeService : IServiceBase
{
    IEnumerable<UserTypeDto> GetAllUserTypes();
    UserTypeDto GetUserTypeById(int id);
    void AddUserType(UserTypeDto userType);
    void UpdateUserType(UserTypeDto userType);
    void DeleteUserTypeById(int userTypeId);
}

public class UserTypeService(IUserTypeRepository userTypeRepository) : IUserTypeService
{
    public IEnumerable<UserTypeDto> GetAllUserTypes()
    {
        return userTypeRepository.GetAllUserTypes();
    }

    public UserTypeDto GetUserTypeById(int id)
    {
        return userTypeRepository.GetUserTypeById(id);
    }

    public void AddUserType(UserTypeDto userType)
    {
        userTypeRepository.AddUserType(userType);
    }

    public void UpdateUserType(UserTypeDto userType)
    {
        userTypeRepository.UpdateUserType(userType);
    }

    public void DeleteUserTypeById(int userTypeId)
    {
        userTypeRepository.DeleteUserTypeById(userTypeId);
    }
}