using TestChat.Server.Models;

namespace TestChat.Server.Hubs;

/// <summary>
/// Interface that represents SignalR messages to be received by clients.
/// </summary>
public interface IChatUser
{
    Task ReceiveMessage(string roomId, string senderId, string message, SentimentAnalysisResult sentiment);
    Task ReceivePublicMessage(string senderId, string message, SentimentAnalysisResult sentiment);
    Task UserOnline(string connectionId, string? userName);
    Task UserJoined(string connectionId);
    Task UserChangedName(string connectionId, string userName);
    Task UserLeft(string connectionId);
    Task ChangeNameResult(bool success, string userName);
}