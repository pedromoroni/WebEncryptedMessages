using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;

namespace WebMessages.Services.Interfaces;

public interface IMessageService
{
    Task SendMessages(List<MessageRequest> messagesRequests);
}
