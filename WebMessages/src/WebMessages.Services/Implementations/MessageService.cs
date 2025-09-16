using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;
using WebMessages.Services.Interfaces;

namespace WebMessages.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task AddMessage(MessageRequest messageRequest)
    {
        // verificacoes dps
        var message = new Message
        {
            Id = Guid.NewGuid(),
            FromUserId = messageRequest.FromUserId,
            ToUserId = messageRequest.ToUserId,
            CipherText = messageRequest.CipherText,
            Nonce = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, // mudar dps
            EphemeralPub = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, // mudar dps
            Status = MessageStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };

        await _messageRepository.AddAsyncMessage(message);
    }
}
