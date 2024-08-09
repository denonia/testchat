using Microsoft.AspNetCore.SignalR;
using TestChat.Server.Services;

namespace TestChat.Server.Hubs;

/// <summary>
/// Main SignalR hub for real-time communication with clients.
/// </summary>
public class ChatHub : Hub<IChatUser>
{
    private readonly ISessionService _sessionService;
    private readonly ITextAnalyticsService _textAnalyticsService;
    private readonly IPersistenceService _persistenceService;

    public ChatHub(ISessionService sessionService, ITextAnalyticsService textAnalyticsService,
        IPersistenceService persistenceService)
    {
        _sessionService = sessionService;
        _textAnalyticsService = textAnalyticsService;
        _persistenceService = persistenceService;
    }

    public override async Task OnConnectedAsync()
    {
        // Notify everyone that a user has joined.
        await Clients.Others.UserJoined(Context.ConnectionId);

        // Send the new user information about every active user.
        foreach (var session in _sessionService.ActiveSessions)
            await Clients.Caller.UserOnline(session.ConnectionId, session.UserName);

        _sessionService.AddUser(Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.UserLeft(Context.ConnectionId);

        _sessionService.RemoveUser(Context.ConnectionId);
    }

    public async Task ChangeName(string userName)
    {
        var success = _sessionService.SetUserName(Context.ConnectionId, userName);

        if (success)
            await Clients.All.UserChangedName(Context.ConnectionId, userName);

        await Clients.Caller.ChangeNameResult(success, userName);
    }

    public async Task SendMessage(string targetId, string message)
    {
        var senderName = _sessionService.FindUser(Context.ConnectionId)!.DisplayName;
        var targetName = _sessionService.FindUser(targetId)?.DisplayName;
        var target = Clients.Client(targetId);
        if (targetName is null)
            return;
        
        var sentiment = await _textAnalyticsService.AnalyzeSentimentAsync(message);
        
        // Send both sender and recipient the message.
        // Sender needs it too as it contains the sentiment analysis results.
        await Clients.Caller.ReceiveMessage(targetId, Context.ConnectionId, message, sentiment);
        await target.ReceiveMessage(Context.ConnectionId, Context.ConnectionId, message, sentiment);

        await _persistenceService.SaveMessageAsync(senderName, targetName, message, sentiment);
    }

    public async Task SendPublicMessage(string message)
    {
        var senderName = _sessionService.FindUser(Context.ConnectionId)!.DisplayName;

        var sentiment = await _textAnalyticsService.AnalyzeSentimentAsync(message);
        
        await Clients.All.ReceivePublicMessage(Context.ConnectionId, message, sentiment);

        await _persistenceService.SaveMessageAsync(senderName, null, message, sentiment);
    }
}