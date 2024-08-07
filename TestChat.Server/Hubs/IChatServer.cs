namespace TestChat.Server.Hubs;

public interface IChatServer
{
    Task ReceiveMessage(string senderId, string message);
    Task UserJoined(string connectionId);
    Task UserChangedName(string connectionId, string userName);
    Task UserLeft(string connectionId);
    Task ChangeNameResult(bool success);
}