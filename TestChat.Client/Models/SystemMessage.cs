namespace TestChat.Client.Models;

public class SystemMessage : Message
{
    public override string Body { get; }

    public SystemMessage(string text)
    {
        Body = text;
    }
}