using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;

namespace WebMessages.Services.Interfaces;

public interface IMessageService
{
    Task AddMessage(MessageRequest messageRequest);
}
