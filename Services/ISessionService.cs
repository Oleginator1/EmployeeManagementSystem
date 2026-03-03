using EmployeeManagementSystem.Models.Entities;

namespace EmployeeManagementSystem.Services
{
    public interface ISessionService
    {
        Task<UserSession> CreateSessionAsync(string userId, string userEmail, string role, HttpContext httpContext);
        Task EndSessionAsync(string sessionToken);
        Task EndAllUserSessionsAsync(string userId);
        Task<UserSession?> GetActiveSessionAsync(string sessionToken);
        Task<List<UserSession>> GetUserSessionsAsync(string userId, int limit = 10);
        Task<List<UserSession>> GetActiveSessionsAsync();
        Task ExpireStaleSessionsAsync(int timeoutMinutes = 60);
    }
}