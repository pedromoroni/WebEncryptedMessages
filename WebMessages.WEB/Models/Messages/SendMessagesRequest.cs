using WebMessages.WEB.Models.Users;

namespace WebMessages.WEB.Models.Messages;

public class SendMessagesRequest
{
    public UserCredentialsRequest UserCredentials { get; set; }
    public List<Message> SentMessages { get; set; } = new List<Message>(); // mensagens encriptadas que o utilizador local enviou
    public List<Message> ReceivedMessages { get; set; } = new List<Message>(); // mensagens encriptadas que o destinatario vai receber
}
