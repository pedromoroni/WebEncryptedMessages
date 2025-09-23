using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Models.Messages;
using WebMessages.Repositories.Interfaces;
using WebMessages.Services.Interfaces;

namespace WebMessages.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository, IDeviceRepository deviceRepository)
    {
        _messageRepository = messageRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task SendMessages(SendMessagesRequest request, UserDto authenticatedUser)
    {
        // verificar se todos os fromDeviceUser pertence a esse id
        List<Device> senderDevices = await _deviceRepository.GetAllDevicesByUserIdAsync(authenticatedUser.Id);

        List<Message> sentMessages = new List<Message>();
        //verificar se os ids dos devices pertencem ao mesmo user
        foreach (MessageRequest message in request.SentMessages)
        {
            if (!senderDevices.Any(d => d.Id == message.FromDeviceId))
                throw new Exception("Invalid From Device Id in SentMessages");

            sentMessages.Add(new Message
            {
                FromDeviceId = message.FromDeviceId,
                ToDeviceId = message.ToDeviceId,
                Received = false,
                CipherText = message.CipherText,
                Nonce = message.Nonce,
                EphemeralPub = message.EphemeralPub,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DeliveredAt = null,
                ReadAt = null
            });
        }

        List<Message> receivedMessages = new List<Message>();
        //verificar se os ids dos devices pertencem ao mesmo user
        foreach (MessageRequest message in request.ReceivedMessages)
        {
            if (!senderDevices.Any(d => d.Id == message.FromDeviceId))
                throw new Exception("Invalid From Device Id in ReceivedMessages");

            receivedMessages.Add(new Message
            {
                FromDeviceId = message.FromDeviceId,
                ToDeviceId = message.ToDeviceId,
                Received = true,
                CipherText = message.CipherText,
                Nonce = message.Nonce,
                EphemeralPub = message.EphemeralPub,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DeliveredAt = null,
                ReadAt = null
            });
        }

        await _messageRepository.AddListMessageAsync(sentMessages);
        await _messageRepository.AddListMessageAsync(receivedMessages);
    }
}
