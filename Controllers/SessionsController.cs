using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{

    [Authorize(Policy = "AdminOnly")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;

        public SessionsController(ApplicationDbContext context, ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        // GET: Sessions
        public async Task<IActionResult> Index(SessionManagementViewModel model)
        {
           
            if (model.PageSize <= 0) model.PageSize = 25;
            if (model.PageNumber <= 0) model.PageNumber = 1;

          
            var query = _context.UserSessions.AsNoTracking().AsQueryable();

           
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var search = model.SearchTerm.ToLower();
                query = query.Where(s =>
                    s.UserEmail.ToLower().Contains(search) ||
                    (s.IpAddress != null && s.IpAddress.Contains(search)));
            }

            
            if (!string.IsNullOrEmpty(model.StatusFilter))
            {
                query = query.Where(s => s.Status == model.StatusFilter);
            }

          
            if (model.DateFrom.HasValue)
            {
                query = query.Where(s => s.LoginTime >= model.DateFrom.Value);
            }
            if (model.DateTo.HasValue)
            {
                var dateTo = model.DateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(s => s.LoginTime <= dateTo);
            }

           
            model.TotalRecords = await query.CountAsync();

            query = query.OrderByDescending(s => s.LoginTime);

            
            var sessions = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            
            model.Sessions = sessions.Select(s => new UserSessionViewModel
            {
                SessionId = s.SessionId,
                UserEmail = s.UserEmail,
                UserRole = s.UserRole,
                LoginTime = s.LoginTime,
                LogoutTime = s.LogoutTime,
                Duration = FormatDuration(s.SessionDurationMinutes),
                IpAddress = s.IpAddress,
                DeviceType = s.DeviceType,
                Browser = s.Browser,
                Status = s.Status,
                IsSuspicious = s.IsSuspicious
            }).ToList();

            // Calculate statistics
            var today = DateTime.Today;
            model.ActiveSessions = await _context.UserSessions
                .CountAsync(s => s.Status == "Active");

            model.TotalSessionsToday = await _context.UserSessions
                .CountAsync(s => s.LoginTime >= today);

            model.UniqueUsersToday = await _context.UserSessions
                .Where(s => s.LoginTime >= today)
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();

            var completedSessions = await _context.UserSessions
                .Where(s => s.SessionDurationMinutes.HasValue)
                .Select(s => s.SessionDurationMinutes!.Value)
                .ToListAsync();

            model.AverageSessionMinutes = completedSessions.Any()
                ? Math.Round(completedSessions.Average(), 1)
                : 0;

            return View(model);
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.UserSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/ForceLogout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceLogout(int id)
        {
            var session = await _context.UserSessions.FindAsync(id);

            if (session == null || session.Status != "Active")
            {
                TempData["ErrorMessage"] = "Session not found or already ended.";
                return RedirectToAction(nameof(Index));
            }

            session.LogoutTime = DateTime.Now;
            session.Status = "ForcedLogout";
            session.SessionDurationMinutes = (int)(session.LogoutTime.Value - session.LoginTime).TotalMinutes;

            _context.Update(session);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Session for {session.UserEmail} has been force-logged out.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Sessions/ForceLogoutUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceLogoutUser(string userId)
        {
            await _sessionService.EndAllUserSessionsAsync(userId);

            TempData["SuccessMessage"] = "All active sessions for this user have been ended.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sessions/ActiveSessions
        public async Task<IActionResult> ActiveSessions()
        {
            var sessions = await _sessionService.GetActiveSessionsAsync();

            var viewModels = sessions.Select(s => new UserSessionViewModel
            {
                SessionId = s.SessionId,
                UserEmail = s.UserEmail,
                UserRole = s.UserRole,
                LoginTime = s.LoginTime,
                Duration = FormatDuration((int)(DateTime.Now - s.LoginTime).TotalMinutes),
                IpAddress = s.IpAddress,
                DeviceType = s.DeviceType,
                Browser = s.Browser,
                Status = s.Status,
                IsSuspicious = s.IsSuspicious
            }).ToList();

            return View(viewModels);
        }

        private string FormatDuration(int? minutes)
        {
            if (!minutes.HasValue || minutes.Value == 0)
                return "-";

            if (minutes.Value < 60)
                return $"{minutes.Value}m";

            var hours = minutes.Value / 60;
            var mins = minutes.Value % 60;

            return mins > 0 ? $"{hours}h {mins}m" : $"{hours}h";
        }
    }
}