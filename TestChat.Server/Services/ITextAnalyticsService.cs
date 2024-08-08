using TestChat.Server.Models;

namespace TestChat.Server.Services;

public interface ITextAnalyticsService
{
    Task<SentimentAnalysisResult?> AnalyzeSentimentAsync(string text);
}