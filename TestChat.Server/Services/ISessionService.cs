using TestChat.Server.Models;

namespace TestChat.Server.Services;

public interface ISessionService
{
    IEnumerable<UserSession> ActiveSessions { get; }

    UserSession? FindUser(string connectionId);
    void AddUser(string connectionId);
    bool SetUserName(string connectionId, string userName);
    void RemoveUser(string connectionId);
}