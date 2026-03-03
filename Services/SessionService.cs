using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSession> CreateSessionAsync(string userId, string userEmail, string role, HttpContext httpContext)
        {
            var sessionToken = Guid.NewGuid().ToString();
            var ipAddress = GetIpAddress(httpContext);
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            var (deviceType, browser) = ParseUserAgent(userAgent);

            var session = new UserSession
            {
                UserId = userId,
                UserEmail = userEmail,
                UserRole = role,
                LoginTime = DateTime.Now,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceType = deviceType,
                Browser = browser,
                Status = "Active",
                SessionToken = sessionToken,
                CreatedDate = DateTime.Now
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            // Store session token in HTTP context for logout
            httpContext.Session.SetString("SessionToken", sessionToken);

            return session;
        }

        public async Task EndSessionAsync(string sessionToken)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.Status == "Active");

            if (session != null)
            {
                session.LogoutTime = DateTime.Now;
                session.Status = "Ended";
                session.SessionDurationMinutes = (int)(session.LogoutTime.Value - session.LoginTime).TotalMinutes;

                _context.Update(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EndAllUserSessionsAsync(string userId)
        {
            var activeSessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.Status == "Active")
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.LogoutTime = DateTime.Now;
                session.Status = "ForcedLogout";
                session.SessionDurationMinutes = (int)(session.LogoutTime.Value - session.LoginTime).TotalMinutes;
                _context.Update(session);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserSession?> GetActiveSessionAsync(string sessionToken)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.Status == "Active");
        }

        public async Task<List<UserSession>> GetUserSessionsAsync(string userId, int limit = 10)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LoginTime)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<UserSession>> GetActiveSessionsAsync()
        {
            return await _context.UserSessions
                .AsNoTracking()
                .Where(s => s.Status == "Active")
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();
        }

        public async Task ExpireStaleSessionsAsync(int timeoutMinutes = 60)
        {
            var cutoffTime = DateTime.Now.AddMinutes(-timeoutMinutes);

            var staleSessions = await _context.UserSessions
                .Where(s => s.Status == "Active" && s.LoginTime < cutoffTime)
                .ToListAsync();

            foreach (var session in staleSessions)
            {
                session.LogoutTime = DateTime.Now;
                session.Status = "Expired";
                session.SessionDurationMinutes = timeoutMinutes;
                _context.Update(session);
            }

            if (staleSessions.Any())
            {
                await _context.SaveChangesAsync();
            }
        }



        private string GetIpAddress(HttpContext httpContext)
        {
            // Try to get real IP from proxy headers first
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private (string deviceType, string browser) ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return ("Unknown", "Unknown");

            userAgent = userAgent.ToLower();

           
            string deviceType = "Desktop";
            if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
                deviceType = "Mobile";
            else if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
                deviceType = "Tablet";

           
            string browser = "Unknown";
            if (userAgent.Contains("edg/") || userAgent.Contains("edge"))
                browser = "Edge";
            else if (userAgent.Contains("chrome"))
                browser = "Chrome";
            else if (userAgent.Contains("firefox"))
                browser = "Firefox";
            else if (userAgent.Contains("safari") && !userAgent.Contains("chrome"))
                browser = "Safari";
            else if (userAgent.Contains("opera") || userAgent.Contains("opr/"))
                browser = "Opera";

            return (deviceType, browser);
        }


    }
}