using Microsoft.AspNetCore.SignalR;
using TestChat.Server.Services;

namespace TestChat.Server.Hubs;

public class ChatHub : Hub<IChatServer>
{
    private readonly ISessionService _sessionService;

    public ChatHub(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.UserJoined(Context.ConnectionId);

        foreach (var session in _sessionService.ActiveSessions)
        {
            await Clients.Caller.UserJoined(session.ConnectionId);
            
            if (!string.IsNullOrEmpty(session.UserName))
                await Clients.Caller.UserChangedName(session.ConnectionId, session.UserName);
        }
        
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
            await Clients.Caller.ChangeNameResult(false);
            return;
        }

        await Clients.Others.UserChangedName(Context.ConnectionId, userName);
        await Clients.Caller.ChangeNameResult(true);
    }

    public async Task SendMessage(string targetId, string message)
    {
        var target = Clients.Client(targetId);
        
        await target.ReceiveMessage(Context.ConnectionId, message);
    }
}