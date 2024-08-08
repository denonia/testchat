using TestChat.Server.Models;

namespace TestChat.Server.Services;

public interface IMessageService
{
    Task SaveMessageAsync(string senderName, string? recipientName, string text, SentimentAnalysisResult sentiment);
}