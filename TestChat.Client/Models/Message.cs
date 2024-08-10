namespace TestChat.Client.Models;

/// <summary>
/// Class that represents a generic chat message.
/// There are two types of messages: <see cref="SystemMessage"/> and <see cref="UserMessage"/>
/// </summary>
public abstract class Message
{
    public abstract string Body { get; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
}