namespace TestChat.Client.Models;

public class UserMessage : Message
{
    public string SenderName { get; }
    public string Text { get; }

    public override string Body => $"{SenderName}: {Text}";
    
    public UserMessage(string senderName, string text)
    {
        SenderName = senderName;
        Text = text;
    }
    
    public UserMessage(string senderName, string text, DateTime sentAt)
    {
        SenderName = senderName;
        Text = text;
        SentAt = sentAt;
    }
}