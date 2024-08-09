namespace TestChat.Client.Models;

/// <summary>
/// Class that represents a basic system message: welcome, user has joined, etc.
/// </summary>
public class SystemMessage : Message
{
    public override string Body { get; }

    public SystemMessage(string text)
    {
        Body = text;
    }
}