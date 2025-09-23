using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using WebMessages.Data;
using WebMessages.Models.DTOs;
using WebMessages.Models.DTOs.Devices;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;
using WebMessages.Services.Interfaces;

namespace WebMessages.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetUserAsync(UserCredentialsRequest userCredentialsRequest, QueryInfo queryInfo)
    {
        if (queryInfo.PageNumber <= 0 || queryInfo.PageSize <= 0)
            throw new InvalidOperationException("Invalid Query Info.");

        var user = await _userRepository.GetByUsernameAsync(userCredentialsRequest.Username, queryInfo);

        if (user == null)
            throw new InvalidOperationException("Invalid Username.");

        if (!BCrypt.Net.BCrypt.Verify(userCredentialsRequest.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid Password.");

        var response = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Devices = user.Devices
                  .Select(d => new DeviceDTO
                  {
                      Id = d.Id,
                      Name = d.Name,
                      PublicKey = d.PublicKey,
                      LastSeen = d.LastSeen,
                      IsActive = d.IsActive,
                      MessagesSent = d.MessagesSent
                      .Select(m => new MessageDTO
                      {
                          FromDeviceId = m.FromDeviceId,
                          ToDeviceId = m.ToDeviceId,
                          CipherText = m.CipherText,
                          Nonce = m.Nonce,
                          EphemeralPub = m.EphemeralPub,
                          Status = m.Status,
                          CreatedAt = m.CreatedAt,
                          DeliveredAt = m.DeliveredAt,
                          ReadAt = m.ReadAt
                      })
                      .ToList(),
                      MessagesReceived = d.MessagesReceived
                      .Select(m => new MessageDTO
                      {
                          FromDeviceId = m.FromDeviceId,
                          ToDeviceId = m.ToDeviceId,
                          CipherText = m.CipherText,
                          Nonce = m.Nonce,
                          EphemeralPub = m.EphemeralPub,
                          Status = m.Status,
                          CreatedAt = m.CreatedAt,
                          DeliveredAt = m.DeliveredAt,
                          ReadAt = m.ReadAt
                      }).ToList(),
                  })
                  .ToList()
        };

        return response;
    }

    public async Task CreateUserAsync(CreateUserRequest userRequest)
    {
        if (userRequest.ConfirmPassword != userRequest.Credentials.Password)
            throw new InvalidOperationException("Passwords do not match.");

        QueryInfo queryInfo = new QueryInfo();

        var existingUser = await _userRepository.GetByUsernameAsync(userRequest.Credentials.Username, queryInfo);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists.");

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = userRequest.Credentials.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRequest.Credentials.Password),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _userRepository.AddAsync(newUser);
    }

    public async Task<List<UserDto>> SearchUsernamesStartingWithAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new List<UserDto>();

        var users = await _userRepository.GetUsersByUsernameAsync(username.Trim().ToLower());

        List<UserDto> userDtos = new List<UserDto>();
        
        foreach (User user in users)
        {
            userDtos.Add(
                new UserDto
                {
                    Id = user.Id,
                    Username = user.Username
                });
        }

        return userDtos;
    }

    public async Task<UserDto> GetUserByDeviceIdAsync(Guid deviceId)
    {
        var user = await _userRepository.GetUserByDeviceIdAsync(deviceId);

        UserDto userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
        };

        return userDto;
    }
}