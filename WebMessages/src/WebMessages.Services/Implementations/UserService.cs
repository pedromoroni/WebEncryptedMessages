using BCrypt.Net;
using WebMessages.Data;
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

    public async Task<UserDto> GetUserAsync(UserCredentialsRequest userCredentialsRequest)
    {
        var user = await _userRepository.GetByUsernameAsync(userCredentialsRequest.Username);

        if (user == null)
            throw new InvalidOperationException("Invalid Username.");

        if (!BCrypt.Net.BCrypt.Verify(userCredentialsRequest.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid Password.");

        var response = new UserDto
        {
            Username = user.Username,
            MessagesSent = user.MessagesSent,
            MessagesReceived = user.MessagesReceived,
            Devices = user.Devices
        };

        return response;
    }

    public async Task CreateUserAsync(CreateUserRequest userRequest)
    {
        if (userRequest.ConfirmPassword != userRequest.Credentials.Password)
            throw new InvalidOperationException("Passwords do not match.");

        var existingUser = await _userRepository.GetByUsernameAsync(userRequest.Credentials.Username);
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
}
