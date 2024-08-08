namespace TestChat.Client.Models;

public class UserMessage : Message
{
    public string SenderName { get; }
    public string Text { get; }
    public SentimentAnalysisResult? Sentiment { get; }

    public override string Body => $"{SenderName}: {Text}";
    
    public UserMessage(string senderName, string text, SentimentAnalysisResult? sentiment)
    {
        SenderName = senderName;
        Text = text;
        Sentiment = sentiment;
    }
    
    public UserMessage(string senderName, string text, SentimentAnalysisResult? sentiment, DateTime sentAt)
    {
        SenderName = senderName;
        Text = text;
        Sentiment = sentiment;
        SentAt = sentAt;
    }
}