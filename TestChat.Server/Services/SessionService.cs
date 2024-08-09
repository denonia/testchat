using TestChat.Server.Models;

namespace TestChat.Server.Services;

/// <summary>
/// Service to keep track of active user sessions.
/// Basic in-memory implementation using a dictionary.
/// </summary>
public class SessionService : ISessionService
{
    private readonly List<UserSession> _activeSessions = [];

    public IEnumerable<UserSession> ActiveSessions => _activeSessions;

    public UserSession? FindUser(string connectionId) =>
        _activeSessions.SingleOrDefault(s => s.ConnectionId == connectionId);

    public void AddUser(string connectionId)
    {
        _activeSessions.Add(new UserSession(connectionId));
    }

    public bool SetUserName(string connectionId, string userName)
    {
        var session = FindUser(connectionId)!;
        userName = userName.Trim();

        // Don't change the username if it's taken or empty
        if (_activeSessions.Any(s => s.UserName == userName) || string.IsNullOrEmpty(userName))
            return false;

        session.UserName = userName;
        return true;
    }

    public void RemoveUser(string connectionId)
    {
        _activeSessions.RemoveAll(s => s.ConnectionId == connectionId);
    }
}