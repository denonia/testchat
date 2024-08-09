using TestChat.Server.Models;

namespace TestChat.Server.Services;

/// <summary>
/// Service for performing sentiment analysis on messages.
/// </summary>
public interface ITextAnalyticsService
{
    Task<SentimentAnalysisResult?> AnalyzeSentimentAsync(string text);
}