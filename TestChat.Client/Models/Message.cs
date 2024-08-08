namespace TestChat.Client.Models;

public abstract class Message
{
    public abstract string Body { get; }
    public DateTime SentAt { get; set; } = DateTime.Now;
}