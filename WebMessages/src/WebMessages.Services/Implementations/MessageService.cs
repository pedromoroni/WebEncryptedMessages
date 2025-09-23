using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Repositories.Interfaces;
using WebMessages.Services.Interfaces;

namespace WebMessages.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task SendMessages(List<MessageRequest> messagesRequests)
    {
        // verificacoes dps
        foreach (MessageRequest messageRequest in messagesRequests)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                FromDeviceId = messageRequest.FromDeviceId,
                ToDeviceId = messageRequest.ToDeviceId,
                CipherText = messageRequest.CipherText,
                Nonce = messageRequest.Nonce,
                EphemeralPub = messageRequest.EphemeralPub,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            // dps fazer uma funcao que adiciona todas e dps salva
            await _messageRepository.AddMessageAsync(message);
        }
    }
}
