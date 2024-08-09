using TestChat.Server.Models;

namespace TestChat.Server.Services;

/// <summary>
/// Service to keep track of active user sessions
/// </summary>
public interface ISessionService
{
    IEnumerable<UserSession> ActiveSessions { get; }

    UserSession? FindUser(string connectionId);
    void AddUser(string connectionId);
    bool SetUserName(string connectionId, string userName);
    void RemoveUser(string connectionId);
}