using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.Entities;

namespace WebMessages.Repositories.Interfaces;

public interface IMessageRepository
{
    Task AddListMessageAsync(List<Message> message);
}
