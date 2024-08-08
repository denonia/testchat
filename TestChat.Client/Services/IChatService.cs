using TestChat.Client.Models;

namespace TestChat.Client.Services;

public interface IChatService : IAsyncDisposable
{
    ChatHistory ActiveChat { get; }
    List<ChatUser> Users { get; }
    
    event Action? OnChange;
    
    Task ConnectAsync();
}