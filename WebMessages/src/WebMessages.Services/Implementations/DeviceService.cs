using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs;
using WebMessages.Models.DTOs.Devices;
using WebMessages.Models.DTOs.UserDevice;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;
using WebMessages.Services.Interfaces;

namespace WebMessages.Services.Implementations;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;

    public DeviceService(IDeviceRepository deviceRepository, IUserRepository userRepository)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
    }

    public async Task RegisterDeviceAsync(UserDeviceRequest userDeviceRequest)
    {
        QueryInfo queryInfo = new QueryInfo();  

        User? user = await _userRepository.GetByUsernameAsync(userDeviceRequest.User.Username, queryInfo);

        if (user == null)
            throw new InvalidOperationException("Invalid Username.");

        if (!BCrypt.Net.BCrypt.Verify(userDeviceRequest.User.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid Password.");

        var existingDevice = await _deviceRepository.GetDeviceByPublicKeyAsync(userDeviceRequest.Device.PublicKey);
        if (existingDevice != null && existingDevice.UserId != user.Id)
            throw new InvalidOperationException("PublicKey already registered in another user.");

        if (await _deviceRepository.GetDeviceByPublicKeyAndUserAsync(userDeviceRequest.Device.PublicKey, user) != null) 
            return;

        Device? device = await _deviceRepository.GetQuantityOfDevicesByUserAsync(user) > 5 
            ? await _deviceRepository.GetOldestSeenDeviceByUserAsync(user) 
            : null;

        if (device != null)
        {
            await _deviceRepository.DeleteDeviceAsync(device);
        }

        if (string.IsNullOrWhiteSpace(userDeviceRequest.Device.Name))
            userDeviceRequest.Device.Name = "Unknown Device";

        Device newDevice = new Device
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = userDeviceRequest.Device.Name,
            PublicKey = userDeviceRequest.Device.PublicKey,
            LastSeen = DateTime.UtcNow,
            IsActive = true,
        };

        await _deviceRepository.AddDeviceAsync(newDevice);
    }

    public async Task<List<DeviceDTO>> GetDevicesByUserIdAsync(Guid userId)
    {
        List<Device> devices = await _deviceRepository.GetAllDevicesByUserIdAsync(userId);

        List<DeviceDTO> devicesDTO = devices
            .Select(c => new DeviceDTO
            {
                Id = c.Id,
                Name = c.Name,
                PublicKey = c.PublicKey,
                LastSeen = c.LastSeen,
                IsActive = c.IsActive
            })
            .ToList();

        return devicesDTO;
    }

    public async Task<DeviceDTO> GetDeviceByPublicKeyAsync(byte[] publicKey)
    {
        Device? device = await _deviceRepository.GetDeviceByPublicKeyAsync(publicKey);

        if (device == null)
            throw new Exception("Device Not Found.");

        DeviceDTO deviceDTO = new DeviceDTO
        {
            Id = device.Id,
            Name = device.Name,
            PublicKey = device.PublicKey,
            LastSeen = device.LastSeen,
            IsActive = device.IsActive
        };

        return deviceDTO;
    }
}
