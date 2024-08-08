using System.ComponentModel.DataAnnotations;

namespace TestChat.Server.Data.Entities;

public class SentimentAnalysis
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; }
    
    [Range(0, 1)] public double PositiveSentiment { get; set; }
    [Range(0, 1)] public double NeutralSentiment { get; set; }
    [Range(0, 1)] public double NegativeSentiment { get; set; }
}