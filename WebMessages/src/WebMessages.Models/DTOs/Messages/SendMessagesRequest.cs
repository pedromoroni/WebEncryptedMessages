using Microsoft.AspNetCore.Identity;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;

namespace WebMessages.Models.Messages;

public class SendMessagesRequest
{
    public UserCredentialsRequest UserCredentials { get; set; }
    public List<MessageRequest> SentMessages { get; set; } = new List<MessageRequest>(); // mensagens encriptadas que o utilizador local enviou
    public List<MessageRequest> ReceivedMessages { get; set; } = new List<MessageRequest>(); // mensagens encriptadas que o destinatario vai receber
}
