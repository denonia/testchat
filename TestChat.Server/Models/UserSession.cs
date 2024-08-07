namespace TestChat.Server.Models;

public class UserSession
{
    public string ConnectionId { get; set; }
    public string? UserName { get; set; }

    public UserSession(string connectionId)
    {
        ConnectionId = connectionId;
    }
}