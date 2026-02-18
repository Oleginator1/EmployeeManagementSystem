using EmployeeManagementSystem.Constants;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{

    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        
        // GET: Users
        public async Task<IActionResult> Index()
        {
            // Always get fresh data from database
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .ToListAsync();

            var viewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Find linked employee
                var employee = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.UserId == user.Id ||
                        e.Email.ToLower() == user.Email!.ToLower());

                
                var lastReset = await _context.AuditLogs
                     .AsNoTracking()
                     .Where(a => a.UserId == user.Email
                         && a.Action == "Login Failed Reset")
                     .OrderByDescending(a => a.Timestamp)
                     .Select(a => a.Timestamp)
                     .FirstOrDefaultAsync();

                
                var failedLoginCount = await _context.AuditLogs
                    .AsNoTracking()
                    .CountAsync(a => a.UserId == user.Email
                        && a.Action == "Login Failed"
                        && a.Timestamp > lastReset);

                // Get last successful login from AuditLog
                var lastLogin = await _context.AuditLogs
                    .AsNoTracking()
                    .Where(a => a.UserId == user.Email
                        && a.Action == "Login Successful")
                    .OrderByDescending(a => a.Timestamp)
                    .Select(a => a.Timestamp)
                    .FirstOrDefaultAsync();

                viewModels.Add(new UserListViewModel
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Roles = roles.ToList(),
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    IsLockedOut = user.LockoutEnd.HasValue
                        && user.LockoutEnd > DateTimeOffset.Now,
                    AccessFailedCount = failedLoginCount,  // From AuditLog, not Identity
                    LastLogin = lastLogin == default ? null : lastLogin,
                    LinkedEmployeeId = employee?.EmployeeId,
                    LinkedEmployeeName = employee?.FullName
                });
            }

            return View(viewModels);
        }

        // POST: Users/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            
            await _userManager.AddToRoleAsync(user, role);

            TempData["SuccessMessage"] = $"Role '{role}' assigned to {user.Email}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Users/UnlockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Reset Identity lockout
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);


            _context.AuditLogs.Add(new Models.Entities.AuditLog
            {
                Action = "Login Failed Reset",
                UserId = user.Email,
                Details = $"Failed login attempts cleared by admin.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User '{user.Email}' unlocked and failed attempts reset.";
            return RedirectToAction(nameof(Index));
        }
    }
}