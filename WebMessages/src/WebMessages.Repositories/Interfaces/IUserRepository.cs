using WebMessages.Models.DTOs;
using WebMessages.Models.Entities;

namespace WebMessages.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, QueryInfo queryInfo);
    Task AddAsync(User user);
    Task<List<User>> GetUsersByUsernameAsync(string username);
    Task<User> GetUserByDeviceIdAsync(Guid DeviceId);
}
