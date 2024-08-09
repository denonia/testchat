using TestChat.Server.Models;

namespace TestChat.Server.Services;

/// <summary>
/// Service for saving messages and sentiment analysis results in storage.
/// </summary>
public interface IPersistenceService
{
    Task SaveMessageAsync(string senderName, string? recipientName, string text, SentimentAnalysisResult sentiment);
}