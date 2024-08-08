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
    public event Action? OnNameChangeSuccess;
    public event Action? OnNameChangeFail;

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

        NotifyStateChanged();
    }

    public async Task ChangeNameAsync(string userName)
    {
        await _hubConnection.SendAsync("ChangeName", userName);
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

        _hubConnection.On<string, string, string, SentimentAnalysisResult>("ReceiveMessage",
            (targetId, senderId, message, sentiment) =>
            {
                var target = Users.SingleOrDefault(u => u.ConnectionId == targetId) ?? Myself;
                var sender = Users.SingleOrDefault(u => u.ConnectionId == senderId) ?? Myself;
                target.History.UserMessage(sender.DisplayName, message, sentiment);
                NotifyStateChanged();
            });

        _hubConnection.On<string, string, SentimentAnalysisResult>("ReceivePublicMessage",
            (senderId, message, sentiment) =>
            {
                var user = Users.SingleOrDefault(u => u.ConnectionId == senderId) ?? Myself;
                PublicChat.UserMessage(user.DisplayName, message, sentiment);
                NotifyStateChanged();
            });
        
        _hubConnection.On<string, string>("UserOnline", (connectionId, userName) =>
        {
            Users.Add(new ChatUser(connectionId, userName));
            NotifyStateChanged();
        });

        _hubConnection.On<string>("UserJoined", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has joined");

            Users.Add(new ChatUser(connectionId));
            NotifyStateChanged();
        });

        _hubConnection.On<string, string>("UserChangedName", (connectionId, userName) =>
        {
            var user = Users.SingleOrDefault(u => u.ConnectionId == connectionId) ?? Myself;
            PublicChat.SystemMessage($"{user.DisplayName} has changed name to {userName}");
            user.History.SystemMessage($"{user.DisplayName} has changed name to {userName}");

            user.UserName = userName;

            NotifyStateChanged();
        });

        _hubConnection.On<string>("UserLeft", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has left");

            Users.RemoveAll(u => u.ConnectionId == connectionId);
            NotifyStateChanged();
        });

        _hubConnection.On<bool, string>("ChangeNameResult", (success, userName) =>
        {
            if (success)
                OnNameChangeSuccess?.Invoke();
            else
                OnNameChangeFail?.Invoke();

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