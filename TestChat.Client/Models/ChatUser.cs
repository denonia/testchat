namespace TestChat.Client.Models;

/// <summary>
/// Class that represents an active chat user.
/// </summary>
public class ChatUser
{
    public string ConnectionId { get; }
    public string? UserName { get; set; }
    public string DisplayName => UserName ?? ConnectionId;
    
    public ChatHistory History { get; } = new();

    public ChatUser(string connectionId)
    {
        ConnectionId = connectionId;
    }
    
    public ChatUser(string connectionId, string? userName)
    {
        ConnectionId = connectionId;
        UserName = userName;
    }
}