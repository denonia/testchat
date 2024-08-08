namespace TestChat.Server.Data.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string SenderName { get; set; }
    public string? RecipientName { get; set; }
    public string Text { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
    
    public SentimentAnalysis? SentimentAnalysis { get; set; }
}