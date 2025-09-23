using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Data;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;

namespace WebMessages.Repositories.Implementations;

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _context;
    public DeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Device?> GetDeviceByPublicKeyAndUserAsync(byte[] publicKey, User user)
    {
        return await _context.Devices
            .Where(d => d.PublicKey == publicKey && d.UserId == user.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<Device?> GetDeviceByPublicKeyAsync(byte[] publicKey)
    {
        return await _context.Devices
            .Where(d => d.PublicKey == publicKey)
            .FirstOrDefaultAsync();
    }


    public async Task<List<Device>> GetAllDevicesByUserIdAsync(Guid userId)
    {
        return await _context.Devices
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> GetQuantityOfDevicesByUserAsync(User user)
    {
        return await _context.Devices
            .Where(c => c.UserId == user.Id)
            .CountAsync();
    }

    public async Task<Device?> GetOldestSeenDeviceByUserAsync(User user)
    {
        return await _context.Devices
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.LastSeen)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteDeviceAsync(Device device)
    {
        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
    }

    public async Task AddDeviceAsync(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
    }
}
