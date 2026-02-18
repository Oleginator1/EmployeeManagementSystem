using EmployeeManagementSystem.Constants;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserService _userService;

        public UserProfileController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        // GET: UserProfile
        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            // Try to find employee by UserId first 
            
            var employee = await FindEmployeeForUserAsync(userId, user.Email!);

            var viewModel = new UserProfileViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,                
                Roles = roles.ToList(),
                EmployeeId = employee?.EmployeeId,
                EmployeeName = employee?.FullName,
                Department = employee?.Department?.Name,
                JobTitle = employee?.JobTitle?.TitleName
            };

            return View(viewModel);
        }

        // GET: UserProfile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var viewModel = new EditUserProfileViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        // POST: UserProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            
            user.PhoneNumber = model.PhoneNumber;

            // Handle email change
            if (user.Email != model.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View(model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.EmailConfirmed = false;

                TempData["InfoMessage"] = "Email updated. Please verify your new email.";
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await LogActionAsync("Updated profile");
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // GET: UserProfile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: UserProfile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userService.GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                await LogActionAsync("Changed password");
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // GET: UserProfile/Security
        public async Task<IActionResult> Security()
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var freshUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (freshUser == null)
                return NotFound();

            // Find the last reset timestamp for this user
            var lastReset = await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == freshUser.Email
                    && a.Action == "Login Failed Reset")
                .OrderByDescending(a => a.Timestamp)
                .Select(a => a.Timestamp)
                .FirstOrDefaultAsync();

            // Get ALL login-related audit logs for this user
            var loginLogs = await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == freshUser.Email &&
                    (a.Action == "Login Successful" ||
                     a.Action == "Login Failed" ||
                     a.Action == "Login Failed Reset" ||
                     a.Action == "Account Locked Out"))
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            // Get recent general activity (non-login)
            var recentActivity = await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == freshUser.Email &&
                    a.Action != "Login Successful" &&
                    a.Action != "Login Failed" &&
                    a.Action != "Login Failed Reset" &&
                    a.Action != "Account Locked Out")
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .Select(a => new UserActivityViewModel
                {
                    Action = a.Action,
                    Timestamp = a.Timestamp,
                    Details = a.Details ?? ""
                })
                .ToListAsync();

            // Count only failures SINCE the last reset
            var totalFailed = loginLogs.Count(l =>
                l.Action == "Login Failed"
                && l.Timestamp > lastReset);

            var totalSuccess = loginLogs.Count(l => l.Action == "Login Successful");

            var lastSuccess = loginLogs
                .Where(l => l.Action == "Login Successful")
                .Select(l => l.Timestamp)
                .FirstOrDefault();

            var lastFailed = loginLogs
                .Where(l => l.Action == "Login Failed")
                .Select(l => l.Timestamp)
                .FirstOrDefault();

            var viewModel = new UserSecurityViewModel
            {
                UserId = freshUser.Id,
                Email = freshUser.Email!,               
                EmailConfirmed = freshUser.EmailConfirmed,
                LockoutEnabled = freshUser.LockoutEnabled,
                LockoutEnd = freshUser.LockoutEnd,
                AccessFailedCount = freshUser.AccessFailedCount,
                TotalFailedAttempts = totalFailed,           // Since last reset
                TotalSuccessfulLogins = totalSuccess,
                LastSuccessfulLogin = lastSuccess == default ? null : lastSuccess,
                LastFailedLogin = lastFailed == default ? null : lastFailed,
                RecentActivity = recentActivity,
                LoginHistory = loginLogs.Take(10).Select(l => new UserActivityViewModel
                {
                    Action = l.Action,
                    Timestamp = l.Timestamp,
                    Details = l.Details ?? ""
                }).ToList()
            };

            return View(viewModel);
        }

       

        // GET: UserProfile/MyEmployee
        public async Task<IActionResult> MyEmployee()
        {
            var userId = _userService.GetCurrentUserId();
            var userEmail = _userService.GetCurrentUserEmail();

            if (userId == null || userEmail == null)
                return RedirectToAction("Login", "Account");

            var employee = await FindEmployeeForUserAsync(userId, userEmail);

            if (employee == null)
            {
                TempData["InfoMessage"] = "No employee record is linked to your account. " +
                    "Your account email must match your employee record email, " +
                    "or an admin must link your account manually.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Details", "Employees", new { id = employee.EmployeeId });
        }

        // GET: UserProfile/LinkEmployee 
        
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> LinkEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Get all employees not yet linked
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.UserId == null || e.UserId == userId)
                .OrderBy(e => e.LastName)
                .ToListAsync();

            ViewData["UserId"] = userId;
            ViewData["UserEmail"] = user.Email;
            ViewData["Employees"] = employees;

            return View();
        }

        // POST: UserProfile/LinkEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> LinkEmployee(string userId, int employeeId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                return NotFound();

            // Remove any existing link for this user
            var previousEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (previousEmployee != null)
            {
                previousEmployee.UserId = null;
                _context.Update(previousEmployee);
            }

            
            employee.UserId = userId;
            _context.Update(employee);

            await _context.SaveChangesAsync();

            await LogActionAsync($"Linked user {user.Email} to employee {employee.FullName}");

            TempData["SuccessMessage"] = $"User '{user.Email}' linked to employee '{employee.FullName}'.";
            return RedirectToAction("Index", "Users");
        }



        #region Helper Methods

       
        private async Task<Employee?> FindEmployeeForUserAsync(string userId, string userEmail)
        {
            
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(e => e.UserId == userId);

           
            if (employee == null)
            {
                employee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.JobTitle)
                    .FirstOrDefaultAsync(e =>
                        e.Email.ToLower() == userEmail.ToLower());
            }

            return employee;
        }

        private async Task LogActionAsync(string action)
        {
            var log = new AuditLog
            {
                Action = action,
                UserId = _userService.GetCurrentUserEmail() ?? "System",
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}