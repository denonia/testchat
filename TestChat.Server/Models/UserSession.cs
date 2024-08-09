namespace TestChat.Server.Models;

/// <summary>
/// Class that represents an active chat user.
/// </summary>
public class UserSession
{
    public string DisplayName => UserName ?? ConnectionId;
    public string ConnectionId { get; set; }
    public string? UserName { get; set; }

    public UserSession(string connectionId)
    {
        ConnectionId = connectionId;
    }
}