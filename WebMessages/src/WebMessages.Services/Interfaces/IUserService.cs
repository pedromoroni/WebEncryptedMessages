using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;

namespace WebMessages.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserAsync(UserCredentialsRequest request);
    Task CreateUserAsync(CreateUserRequest request);
}
