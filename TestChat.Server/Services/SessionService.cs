using TestChat.Server.Models;

namespace TestChat.Server.Services;

public class SessionService : ISessionService
{
    private readonly List<UserSession> _activeSessions = [];

    public IEnumerable<UserSession> ActiveSessions => _activeSessions;

    private UserSession? FindUser(string connectionId) =>
        _activeSessions.SingleOrDefault(s => s.ConnectionId == connectionId);

    public void AddUser(string connectionId)
    {
        _activeSessions.Add(new UserSession(connectionId));
    }

    public bool SetUserName(string connectionId, string userName)
    {
        var session = FindUser(connectionId);
        if (_activeSessions.Any(s => s.UserName == userName) || session is null)
            return false;

        session.UserName = userName;
        return true;
    }

    public void RemoveUser(string connectionId)
    {
        _activeSessions.RemoveAll(s => s.ConnectionId == connectionId);
    }
}