namespace TestChat.Server.Models;

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