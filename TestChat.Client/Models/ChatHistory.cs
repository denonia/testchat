namespace TestChat.Client.Models;

public class ChatHistory
{
    public List<Message> Messages { get; } = [];

    public void SystemMessage(string text) => Messages.Add(new SystemMessage(text));
    
    public void UserMessage(string senderName, string text) => Messages.Add(new UserMessage(senderName, text));
}