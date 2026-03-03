using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagementSystem.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ISessionService _sessionService;
        private readonly ApplicationDbContext _context;

        public LogoutModel(
            SignInManager<IdentityUser> signInManager,
            ILogger<LogoutModel> logger,
            ISessionService sessionService,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _sessionService = sessionService;
            _context = context;
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            // End the session before signing out
            var sessionToken = HttpContext.Session.GetString("SessionToken");
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await _sessionService.EndSessionAsync(sessionToken);
            }

            // Log logout to audit log
            if (User.Identity?.IsAuthenticated == true)
            {
                var userEmail = User.Identity.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    await LogLogoutEventAsync(userEmail);
                }
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }

        private async Task LogLogoutEventAsync(string email)
        {
            try
            {
                _context.AuditLogs.Add(new AuditLog
                {
                    Action = "Logout",
                    UserId = email,
                    Details = "User logged out",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log logout event");
            }
        }
    }
}