using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Users;

namespace WebMessages.Models.DTOs.Messages;

public class ReceiveMessagesRequest
{
    public UserCredentialsRequest UserCredentials { get; set; }
    public byte[] PublicKey { get; set; }
}
