using Microsoft.AspNetCore.SignalR.Client;
using TestChat.Client.Models;

namespace TestChat.Client.Services;

/// <summary>
/// Service that provides functionality for real-time interaction with the chat server.
/// SignalR implementation.
/// </summary>
public class ChatService : IChatService
{
    private const string WelcomeMessage =
        "Welcome to the public chat. The list of online users is shown on the left. Click on their names to message them privately.";

    private readonly HubConnection _hubConnection;

    public ChatHistory ActiveChat => ActiveUser?.History ?? PublicChat;
    public ChatHistory PublicChat { get; } = new();

    public ChatUser? Myself { get; private set; }
    public ChatUser? ActiveUser { get; private set; }
    public List<ChatUser> Users { get; } = [];

    public event Action? OnChange;
    public event Action? OnChatRoomChange;
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

        try
        {
            await _hubConnection.StartAsync();
            Myself = new ChatUser(_hubConnection.ConnectionId!);
            PublicChat.SystemMessage(WelcomeMessage);
        }
        catch (HttpRequestException)
        {
            PublicChat.SystemMessage("Failed to connect to the server.");
        }
    }

    public async Task SendMessageAsync(string text)
    {
        if (ActiveChat == PublicChat)
            await _hubConnection.SendAsync("SendPublicMessage", text);
        else
            await _hubConnection.SendAsync("SendMessage", ActiveUser!.ConnectionId, text);

        StateChanged();
    }

    public async Task ChangeNameAsync(string userName)
    {
        await _hubConnection.SendAsync("ChangeName", userName);
    }

    public void ChangeRoom(string? userId = null)
    {
        ActiveUser = Users.SingleOrDefault(u => u.ConnectionId == userId);
        OnChatRoomChange?.Invoke();
        StateChanged();
    }

    private void RegisterHandlers()
    {
        _hubConnection.On<string, string, string, SentimentAnalysisResult>("ReceiveMessage",
            (targetId, senderId, message, sentiment) =>
            {
                var target = FindUser(targetId);
                var sender = FindUser(senderId);
                target.History.UserMessage(sender.DisplayName, message, sentiment);
                StateChanged();
            });

        _hubConnection.On<string, string, SentimentAnalysisResult>("ReceivePublicMessage",
            (senderId, message, sentiment) =>
            {
                var user = FindUser(senderId);
                PublicChat.UserMessage(user.DisplayName, message, sentiment);
                StateChanged();
            });

        _hubConnection.On<string, string>("UserOnline", (connectionId, userName) =>
        {
            Users.Add(new ChatUser(connectionId, userName));
            StateChanged();
        });

        _hubConnection.On<string>("UserJoined", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has joined");

            Users.Add(new ChatUser(connectionId));
            StateChanged();
        });

        _hubConnection.On<string, string>("UserChangedName", (connectionId, userName) =>
        {
            var user = FindUser(connectionId);
            PublicChat.SystemMessage($"{user.DisplayName} has changed name to {userName}");
            user.History.SystemMessage($"{user.DisplayName} has changed name to {userName}");

            user.UserName = userName;

            StateChanged();
        });

        _hubConnection.On<string>("UserLeft", connectionId =>
        {
            PublicChat.SystemMessage($"{connectionId} has left");

            Users.RemoveAll(u => u.ConnectionId == connectionId);
            StateChanged();
        });

        _hubConnection.On<bool, string>("ChangeNameResult", (success, userName) =>
        {
            if (success)
                OnNameChangeSuccess?.Invoke();
            else
                OnNameChangeFail?.Invoke();

            StateChanged();
        });
    }

    private void StateChanged()
    {
        // Mark all messages in active chat as read
        ActiveChat.MarkAllAsRead();
        
        OnChange?.Invoke();
    }

    private ChatUser FindUser(string targetId) => Users.SingleOrDefault(u => u.ConnectionId == targetId) ?? Myself!;

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}