namespace TestChat.Server.Models;

/// <summary>
/// Class that represents results of sentiment analysis performed on a user's message.
/// </summary>
public class SentimentAnalysisResult
{
    public double PositiveScore { get; }
    public double NeutralScore { get; }
    public double NegativeScore { get; }

    public SentimentAnalysisResult(double positiveScore, double neutralScore, double negativeScore)
    {
        PositiveScore = positiveScore;
        NeutralScore = neutralScore;
        NegativeScore = negativeScore;
    }
}