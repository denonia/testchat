using TestChat.Client.Models;

namespace TestChat.Client.Services;

public interface IChatService : IAsyncDisposable
{
    ChatHistory ActiveChat { get; }
    ChatUser? Myself { get; }
    ChatUser? ActiveUser { get; }
    List<ChatUser> Users { get; }
    
    event Action? OnChange;
    
    Task ConnectAsync();
    Task SendMessageAsync(string text);

    void ChangeRoom(string? userId = null);
}