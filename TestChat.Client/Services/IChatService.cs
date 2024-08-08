using TestChat.Client.Models;

namespace TestChat.Client.Services;

public interface IChatService : IAsyncDisposable
{
    ChatHistory ActiveChat { get; }
    ChatUser? Myself { get; }
    ChatUser? ActiveUser { get; }
    List<ChatUser> Users { get; }
    
    event Action? OnChange;
    event Action? OnNameChangeSuccess;
    event Action? OnNameChangeFail;
    
    Task ConnectAsync();
    Task SendMessageAsync(string text);
    Task ChangeNameAsync(string userName);

    void ChangeRoom(string? userId = null);
}