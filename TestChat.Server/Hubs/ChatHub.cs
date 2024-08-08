using Microsoft.AspNetCore.SignalR;
using TestChat.Server.Services;

namespace TestChat.Server.Hubs;

public class ChatHub : Hub<IChatServer>
{
    private readonly ISessionService _sessionService;
    private readonly ITextAnalyticsService _textAnalyticsService;
    private readonly IMessageService _messageService;

    public ChatHub(ISessionService sessionService, ITextAnalyticsService textAnalyticsService,
        IMessageService messageService)
    {
        _sessionService = sessionService;
        _textAnalyticsService = textAnalyticsService;
        _messageService = messageService;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Others.UserJoined(Context.ConnectionId);

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

        if (!success)
        {
            await Clients.Caller.ChangeNameResult(false, userName);
            return;
        }

        await Clients.All.UserChangedName(Context.ConnectionId, userName);
        await Clients.Caller.ChangeNameResult(true, userName);
    }

    public async Task SendMessage(string targetId, string message)
    {
        var senderName = _sessionService.FindUser(Context.ConnectionId)?.DisplayName;
        var targetName = _sessionService.FindUser(targetId)?.DisplayName;
        if (senderName is null || targetName is null)
            return;

        var target = Clients.Client(targetId);
        var sentiment = await _textAnalyticsService.AnalyzeSentimentAsync(message);
        await Clients.Caller.ReceiveMessage(targetId, Context.ConnectionId, message, sentiment);
        await target.ReceiveMessage(Context.ConnectionId, Context.ConnectionId, message, sentiment);

        await _messageService.SaveMessageAsync(senderName, targetName, message, sentiment);
    }

    public async Task SendPublicMessage(string message)
    {
        var senderName = _sessionService.FindUser(Context.ConnectionId)?.DisplayName;
        if (senderName is null)
            return;

        var sentiment = await _textAnalyticsService.AnalyzeSentimentAsync(message);
        await Clients.All.ReceivePublicMessage(Context.ConnectionId, message, sentiment);

        await _messageService.SaveMessageAsync(senderName, null, message, sentiment);
    }
}