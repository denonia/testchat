using Microsoft.AspNetCore.SignalR.Client;
using TestChat.Client.Models;

namespace TestChat.Client.Services;

public class ChatService : IChatService
{
    private readonly HubConnection? _hubConnection;

    public ChatHistory ActiveChat => ActiveUser?.History ?? PublicChat;
    public ChatHistory PublicChat { get; } = new();
    
    public ChatUser? Myself { get; private set; }
    public ChatUser? ActiveUser { get; private set; }
    public List<ChatUser> Users { get; } = [];

    public event Action? OnChange;

    public ChatService(IConfiguration configuration)
    {
        var url = configuration["ChatHubUrl"] ?? throw new Exception("Chat Hub URL not found in config.");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
    }

    public async Task ConnectAsync()
    {
        RegisterHandlers();

        await _hubConnection.StartAsync();
        Myself = new ChatUser(_hubConnection.ConnectionId);
    }

    public async Task SendMessageAsync(string text)
    {
        if (ActiveChat == PublicChat)
            await _hubConnection.SendAsync("SendPublicMessage", text);
        else
            await _hubConnection.SendAsync("SendMessage", ActiveUser.ConnectionId, text);
        
        ActiveChat.UserMessage(Myself.DisplayName, text);
        NotifyStateChanged();
    }

    public void ChangeRoom(string? userId = null)
    {
        ActiveUser = Users.SingleOrDefault(u => u.ConnectionId == userId);
        NotifyStateChanged();
    }

    private void RegisterHandlers()
    {
        if (_hubConnection is null)
            return;
        
        _hubConnection.On<string, string>("ReceiveMessage", (senderId, message) =>
        {
            var user = Users.Single(u => u.ConnectionId == senderId);
            user.History.UserMessage(user.DisplayName, message);
            NotifyStateChanged();
        });
        
        _hubConnection.On<string, string>("ReceivePublicMessage", (senderId, message) =>
        {
            var user = Users.Single(u => u.ConnectionId == senderId);
            PublicChat.UserMessage(user.DisplayName, message);
            NotifyStateChanged();
        });
        

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