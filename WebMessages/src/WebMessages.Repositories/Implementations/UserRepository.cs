using Microsoft.EntityFrameworkCore;
using WebMessages.Data;
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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
                .Where(c => c.Username == username)
                .Where(c => !c.IsDeleted)
                .Include(c => c.MessagesReceived)
                .Include(c => c.MessagesSent)
                .FirstOrDefaultAsync();
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
