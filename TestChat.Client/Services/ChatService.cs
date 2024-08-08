using Microsoft.AspNetCore.SignalR.Client;
using TestChat.Client.Models;

namespace TestChat.Client.Services;

public class ChatService : IChatService
{
    private readonly HubConnection? _hubConnection;

    public ChatHistory ActiveChat { get; private set; }
    public ChatHistory PublicChat { get; } = new();
    public List<ChatUser> Users { get; } = [];

    public event Action? OnChange;

    public ChatService(IConfiguration configuration)
    {
        ActiveChat = PublicChat;

        var url = configuration["ChatHubUrl"] ?? throw new Exception("Chat Hub URL not found in config.");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
    }

    public async Task ConnectAsync()
    {
        RegisterHandlers();

        await _hubConnection.StartAsync();
    }

    private void RegisterHandlers()
    {
        if (_hubConnection is null)
            return;

        _hubConnection.On<string>("UserJoined", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has joined");

            Users.Add(new ChatUser(connectionId));
            NotifyStateChanged();
        });

        _hubConnection.On<string>("UserLeft", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has left");

            Users.RemoveAll(u => u.ConnectionId == connectionId);
            NotifyStateChanged();
        });
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }
}