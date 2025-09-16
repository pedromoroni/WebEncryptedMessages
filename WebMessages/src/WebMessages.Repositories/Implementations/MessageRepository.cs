using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Data;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;

namespace WebMessages.Repositories.Implementations;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;
    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsyncMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }
}
