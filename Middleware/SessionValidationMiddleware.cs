using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementSystem.Middleware
{
  
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, ISessionService sessionService, SignInManager<IdentityUser> signInManager)
        {           
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sessionToken = context.Session.GetString("SessionToken");
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    // Check if session is still active in database
                    var session = await sessionService.GetActiveSessionAsync(sessionToken);

                    if (session == null){
                        _logger.LogWarning($"Invalid session detected for user. Forcing logout. Token: {sessionToken}");
                                         
                        await signInManager.SignOutAsync();

                        context.Session.Clear();

                        // Redirect to login with message
                        context.Response.Redirect("/Identity/Account/Login?forcedLogout=true");
                        return;
                    }
                }
                else
                {                    
                    _logger.LogWarning("Authenticated user has no session token");
                }
            }

            await _next(context);
        }
    }
}