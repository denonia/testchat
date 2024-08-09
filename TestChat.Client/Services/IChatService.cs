using TestChat.Client.Models;

namespace TestChat.Client.Services;

/// <summary>
/// Service that provides functionality for real-time interaction with the chat server.
/// </summary>
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