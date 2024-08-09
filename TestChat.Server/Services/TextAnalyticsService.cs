using Azure;
using Azure.AI.TextAnalytics;
using TestChat.Server.Models;

namespace TestChat.Server.Services;

/// <summary>
/// Service for performing sentiment analysis on messages.
/// Azure Cognitive Services implementation.
/// </summary>
public class TextAnalyticsService : ITextAnalyticsService
{
    private readonly TextAnalyticsClient _client;

    public TextAnalyticsService(IConfiguration configuration)
    {
        var url = configuration["AzureTextAnalyticsServiceUrl"] ??
                  throw new Exception("Azure Text Analytics service URL is not specified in configuration.");
        var key = configuration["AzureTextAnalyticsServiceApiKey"] ??
                  throw new Exception("Azure Text Analytics service API key is not specified in configuration.");
        
        var endpoint = new Uri(url);
        var credential = new AzureKeyCredential(key);
        _client = new TextAnalyticsClient(endpoint, credential);
    }

    public async Task<SentimentAnalysisResult?> AnalyzeSentimentAsync(string text)
    {
        var sentiment = await _client.AnalyzeSentimentAsync(text);
        if (!sentiment.HasValue)
            return null;

        var scores = sentiment.Value.ConfidenceScores;
        return new SentimentAnalysisResult(scores.Positive, scores.Neutral, scores.Negative);
    }
}