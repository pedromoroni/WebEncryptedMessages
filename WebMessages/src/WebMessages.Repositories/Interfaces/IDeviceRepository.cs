using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.Entities;

namespace WebMessages.Repositories.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetDeviceByPublicKeyAndUserAsync(byte[] publicKey, User user);
    Task<Device?> GetDeviceByPublicKeyAsync(byte[] publicKey);
    Task<List<Device>> GetAllDevicesByUserIdAsync(Guid userId);
    Task<int> GetQuantityOfDevicesByUserAsync(User user);
    Task<Device?> GetOldestSeenDeviceByUserAsync(User user);
    Task DeleteDeviceAsync(Device device);
    Task AddDeviceAsync(Device device);
}
