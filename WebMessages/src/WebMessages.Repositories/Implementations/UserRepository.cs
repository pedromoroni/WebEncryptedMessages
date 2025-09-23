using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebMessages.Data;
using WebMessages.Models.DTOs;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;

namespace WebMessages.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, QueryInfo queryInfo)
    {
        var skip = (queryInfo.PageNumber - 1) * queryInfo.PageSize;

        var user = await _context.Users
            .Where(u => u.Username == username && !u.IsDeleted)
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                PasswordHash = u.PasswordHash,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                IsDeleted = u.IsDeleted
            })
            .FirstOrDefaultAsync();

        if (user == null) return null;

        var devices = await _context.Devices
            .Where(d => d.UserId == user.Id)
            .Select(d => new Device
            {
                Id = d.Id,
                UserId = d.UserId,
                Name = d.Name,
                PublicKey = d.PublicKey,
                LastSeen = d.LastSeen,
                IsActive = d.IsActive
            })
            .ToListAsync();

        foreach (var device in devices)
        {
            var messagesSent = new List<Message>();

            var recipientIds = await _context.Messages
                .Where(m => m.FromDeviceId == device.Id && !m.Received)
                .Select(m => m.ToDeviceId)
                .Distinct()
                .ToListAsync();

            foreach (var recipientId in recipientIds)
            {
                var msgs = await _context.Messages
                    .Where(m => m.FromDeviceId == device.Id && m.ToDeviceId == recipientId && !m.Received)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(queryInfo.PageSize)
                    .ToListAsync();

                messagesSent.AddRange(msgs);
            }

            device.MessagesSent = messagesSent;

            var messagesReceived = new List<Message>();

            var senderIds = await _context.Messages
                .Where(m => m.ToDeviceId == device.Id)
                .Select(m => m.FromDeviceId)
                .Distinct()
                .ToListAsync();

            foreach (var senderId in senderIds)
            {
                var msgs = await _context.Messages
                    .Where(c => c.Received)
                    .Where(m => m.ToDeviceId == device.Id && m.FromDeviceId == senderId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(queryInfo.PageSize)
                    .ToListAsync();

                messagesReceived.AddRange(msgs);
            }

            device.MessagesReceived = messagesReceived;
        }

        user.Devices = devices;

        return user;
    }


    public async Task<List<User>> GetUsersByUsernameAsync(string username)
    {
        return await _context.Users
            .Where (c => c.Username.StartsWith(username))
            .Where(c => !c.IsDeleted)
            .Include(c => c.Devices)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByDeviceIdAsync(Guid DeviceId)
    {
        return await _context.Users.FirstOrDefaultAsync(c => c.Devices.Any(c => c.Id == DeviceId));
    }
}
