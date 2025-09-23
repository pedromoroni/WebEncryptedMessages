using WebMessages.Models.DTOs;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;

namespace WebMessages.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserAsync(UserCredentialsRequest request, QueryInfo queryInfo);
    Task CreateUserAsync(CreateUserRequest request);
    Task<List<UserDto>> SearchUsernamesStartingWithAsync(string username);
    Task<UserDto> GetUserByDeviceIdAsync(Guid deviceId);
}
