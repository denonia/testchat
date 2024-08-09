namespace TestChat.Client.Models;

/// <summary>
/// Class that represents the history of messages sent in a chat room.
/// </summary>
public class ChatHistory
{
    public List<Message> Messages { get; } = [];

    public void SystemMessage(string text) => Messages.Add(new SystemMessage(text));
    
    public void UserMessage(string senderName, string text, SentimentAnalysisResult sentiment) => 
        Messages.Add(new UserMessage(senderName, text, sentiment));
}