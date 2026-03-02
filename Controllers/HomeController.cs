using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeIndexViewModel
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            };

            if (viewModel.IsAuthenticated)
            {
                viewModel.UserName = User.Identity?.Name;
                viewModel.IsAdmin = User.IsInRole("Admin");
                viewModel.IsManager = User.IsInRole("Manager");
                viewModel.IsEmployee = User.IsInRole("Employee");

                if (viewModel.IsAdmin)
                    viewModel.UserRole = "Administrator";
                else if (viewModel.IsManager)
                    viewModel.UserRole = "Manager";
                else if (viewModel.IsEmployee)
                    viewModel.UserRole = "Employee";

               
                if (viewModel.IsAdmin || viewModel.IsManager)
                {
                    await LoadAdminStatisticsAsync(viewModel);
                }

                
                var userEmail = _userService.GetCurrentUserEmail();
                if (userEmail != null)
                {
                    var employee = await _context.Employees
                        .Include(e => e.Department)
                        .Include(e => e.JobTitle)
                        .FirstOrDefaultAsync(e => e.Email.ToLower() == userEmail.ToLower());

                    if (employee != null)
                    {
                        viewModel.EmployeeId = employee.EmployeeId;
                        viewModel.EmployeeName = employee.FullName;
                        viewModel.Department = employee.Department?.Name;
                        viewModel.JobTitle = employee.JobTitle?.TitleName;
                        viewModel.HireDate = employee.HireDate;
                        viewModel.YearsOfService = DateTime.Now.Year - employee.HireDate.Year;
                    }
                }

               
                viewModel.RecentActivities = await GetRecentActivitiesAsync();
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Helper Methods

        private async Task LoadAdminStatisticsAsync(HomeIndexViewModel viewModel)
        {

            viewModel.TotalEmployees = await _context.Employees
                .AsNoTracking()
                .CountAsync();

            viewModel.ActiveEmployees = await _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.IsActive);

            viewModel.TotalDepartments = await _context.Departments
                .AsNoTracking()
                .CountAsync();

            viewModel.TotalJobTitles = await _context.JobTitles
                .AsNoTracking()
                .CountAsync();

          
            if (viewModel.TotalEmployees > 0)
            {
                viewModel.AverageSalary = await _context.Employees
                    .AsNoTracking()
                    .AverageAsync(e => e.Salary);
            }

           
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            viewModel.NewEmployeesThisMonth = await _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.HireDate >= firstDayOfMonth);

            
            var employeesByDept = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Where(e => e.IsActive && e.Department != null) 
                .GroupBy(e => e.Department!.Name)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            viewModel.EmployeesByDepartment = employeesByDept
                .ToDictionary(x => x.Department, x => x.Count);
        }

        private async Task<List<RecentActivityItem>> GetRecentActivitiesAsync()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .ToListAsync();

            return logs.Select(log => new RecentActivityItem
            {
                Action = log.Action,
                User = log.UserId,
                Timestamp = log.Timestamp,
                Icon = GetIconForAction(log.Action),
                BadgeColor = GetBadgeColorForAction(log.Action)
            }).ToList();
        }

        private string GetIconForAction(string action)
        {
            if (action.Contains("Created", StringComparison.OrdinalIgnoreCase))
                return "bi-plus-circle";
            if (action.Contains("Updated", StringComparison.OrdinalIgnoreCase))
                return "bi-pencil";
            if (action.Contains("Deleted", StringComparison.OrdinalIgnoreCase))
                return "bi-trash";
            if (action.Contains("Login", StringComparison.OrdinalIgnoreCase))
                return "bi-box-arrow-in-right";
            return "bi-info-circle";
        }

        private string GetBadgeColorForAction(string action)
        {
            if (action.Contains("Created", StringComparison.OrdinalIgnoreCase))
                return "success";
            if (action.Contains("Updated", StringComparison.OrdinalIgnoreCase))
                return "info";
            if (action.Contains("Deleted", StringComparison.OrdinalIgnoreCase))
                return "danger";
            if (action.Contains("Login", StringComparison.OrdinalIgnoreCase))
                return "primary";
            return "secondary";
        }

        #endregion
    }
}