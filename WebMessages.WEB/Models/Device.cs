namespace WebMessages.WEB.Models;

public class Device
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
    public bool IsActive { get; set; }
    
    public List<Message> MessagesSent { get; set; } = new();
    public List<Message> MessagesReceived { get; set; } = new();
}