using EmployeeManagementSystem.Constants;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
   
    [Authorize(Policy = "AdminOnly")]
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public AuditLogsController(ApplicationDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

       
        public async Task<IActionResult> Index(AuditLogSearchViewModel searchModel)
        {
            
            var query = _context.AuditLogs.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var search = searchModel.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Action.ToLower().Contains(search) ||
                    (a.Details != null && a.Details.ToLower().Contains(search))
                );
            }

            
            if (!string.IsNullOrEmpty(searchModel.UserFilter))
            {
                query = query.Where(a => a.UserId == searchModel.UserFilter);
            }

            
            if (!string.IsNullOrEmpty(searchModel.ActionTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.ActionTypeFilter));
            }

            
            if (!string.IsNullOrEmpty(searchModel.EntityTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.EntityTypeFilter));
            }

           
            if (searchModel.DateFrom.HasValue)
            {
                var dateFrom = searchModel.DateFrom.Value.Date;
                query = query.Where(a => a.Timestamp >= dateFrom);
            }
            if (searchModel.DateTo.HasValue)
            {
                var dateTo = searchModel.DateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(a => a.Timestamp <= dateTo);
            }

            
            searchModel.TotalRecords = await query.CountAsync();

            
            query = ApplySorting(query, searchModel.SortBy, searchModel.SortOrder);

            
            var logs = await query
                .Skip((searchModel.PageNumber - 1) * searchModel.PageSize)
                .Take(searchModel.PageSize)
                .ToListAsync();

            
            var employeeIds = logs.Where(l => l.EmployeeId.HasValue).Select(l => l.EmployeeId.Value).Distinct();
            var employees = await _context.Employees
                .Where(e => employeeIds.Contains(e.EmployeeId))
                .ToDictionaryAsync(e => e.EmployeeId, e => e.FullName);

            
            searchModel.AuditLogs = logs.Select(log => new AuditLogViewModel
            {
                Id = log.Id,
                Action = log.Action,
                UserId = log.UserId,
                Timestamp = log.Timestamp,
                Details = log.Details,
                EmployeeId = log.EmployeeId,
                EmployeeName = log.EmployeeId.HasValue && employees.ContainsKey(log.EmployeeId.Value)
                    ? employees[log.EmployeeId.Value]
                    : null,
                ActionType = ExtractActionType(log.Action),
                EntityType = ExtractEntityType(log.Action)
            }).ToList();

           
            await PopulateDropdownsAsync(searchModel);

            
            var allLogs = _context.AuditLogs.AsQueryable();
            searchModel.TotalActions = await allLogs.CountAsync();

            var today = DateTime.Today;
            searchModel.ActionsToday = await allLogs.CountAsync(a => a.Timestamp >= today);

            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            searchModel.ActionsThisWeek = await allLogs.CountAsync(a => a.Timestamp >= weekStart);

            return View(searchModel);
        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.AuditLogs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (log == null)
            {
                return NotFound();
            }

            
            string? employeeName = null;
            if (log.EmployeeId.HasValue)
            {
                var employee = await _context.Employees.FindAsync(log.EmployeeId.Value);
                employeeName = employee?.FullName;
            }

            var viewModel = new AuditLogViewModel
            {
                Id = log.Id,
                Action = log.Action,
                UserId = log.UserId,
                Timestamp = log.Timestamp,
                Details = log.Details,
                EmployeeId = log.EmployeeId,
                EmployeeName = employeeName,
                ActionType = ExtractActionType(log.Action),
                EntityType = ExtractEntityType(log.Action)
            };

            return View(viewModel);
        }

        
        public async Task<IActionResult> ExportCsv(AuditLogSearchViewModel searchModel)
        {
            
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var search = searchModel.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Action.ToLower().Contains(search) ||
                    (a.Details != null && a.Details.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrEmpty(searchModel.UserFilter))
            {
                query = query.Where(a => a.UserId == searchModel.UserFilter);
            }

            if (!string.IsNullOrEmpty(searchModel.ActionTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.ActionTypeFilter));
            }

            if (!string.IsNullOrEmpty(searchModel.EntityTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.EntityTypeFilter));
            }

            if (searchModel.DateFrom.HasValue)
            {
                var dateFrom = searchModel.DateFrom.Value.Date;
                query = query.Where(a => a.Timestamp >= dateFrom);
            }

            if (searchModel.DateTo.HasValue)
            {
                var dateTo = searchModel.DateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(a => a.Timestamp <= dateTo);
            }

            query = ApplySorting(query, searchModel.SortBy, searchModel.SortOrder);

            var logs = await query.ToListAsync();

            
            var employeeIds = logs.Where(l => l.EmployeeId.HasValue).Select(l => l.EmployeeId.Value).Distinct();
            var employees = await _context.Employees
                .Where(e => employeeIds.Contains(e.EmployeeId))
                .ToDictionaryAsync(e => e.EmployeeId, e => e.FullName);

            // Build CSV content
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("ID,Timestamp,User,Action,Entity Type,Employee,Details");

            foreach (var log in logs)
            {
                var employeeName = log.EmployeeId.HasValue && employees.ContainsKey(log.EmployeeId.Value)
                    ? employees[log.EmployeeId.Value]
                    : "";

                csv.AppendLine($"{log.Id}," +
                              $"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\"," +
                              $"\"{EscapeCsv(log.UserId)}\"," +
                              $"\"{EscapeCsv(log.Action)}\"," +
                              $"\"{EscapeCsv(ExtractEntityType(log.Action))}\"," +
                              $"\"{EscapeCsv(employeeName)}\"," +
                              $"\"{EscapeCsv(log.Details ?? "")}\"");
            }

            var fileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var cutoffDate = DateTime.Now.AddDays(-90);
            var oldLogs = await _context.AuditLogs
                .Where(a => a.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cleared {oldLogs.Count} audit log entries older than 90 days.";
            }
            else
            {
                TempData["InfoMessage"] = "No audit log entries older than 90 days found.";
            }

            return RedirectToAction(nameof(Index));
        }






        public async Task<IActionResult> ExportPdf(AuditLogSearchViewModel searchModel)
        {
            
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var search = searchModel.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Action.ToLower().Contains(search) ||
                    (a.Details != null && a.Details.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrEmpty(searchModel.UserFilter))
            {
                query = query.Where(a => a.UserId == searchModel.UserFilter);
            }

            if (!string.IsNullOrEmpty(searchModel.ActionTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.ActionTypeFilter));
            }

            if (!string.IsNullOrEmpty(searchModel.EntityTypeFilter))
            {
                query = query.Where(a => a.Action.Contains(searchModel.EntityTypeFilter));
            }

            if (searchModel.DateFrom.HasValue)
            {
                var dateFrom = searchModel.DateFrom.Value.Date;
                query = query.Where(a => a.Timestamp >= dateFrom);
            }

            if (searchModel.DateTo.HasValue)
            {
                var dateTo = searchModel.DateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(a => a.Timestamp <= dateTo);
            }

            query = ApplySorting(query, searchModel.SortBy, searchModel.SortOrder);

            var logs = await query.ToListAsync();

            // Get employee names
            var employeeIds = logs.Where(l => l.EmployeeId.HasValue).Select(l => l.EmployeeId.Value).Distinct();
            var employees = await _context.Employees
                .Where(e => employeeIds.Contains(e.EmployeeId))
                .ToDictionaryAsync(e => e.EmployeeId, e => e.FullName);

            // Map to view models
            var viewModels = logs.Select(log => new AuditLogViewModel
            {
                Id = log.Id,
                Action = log.Action,
                UserId = log.UserId,
                Timestamp = log.Timestamp,
                Details = log.Details,
                EmployeeId = log.EmployeeId,
                EmployeeName = log.EmployeeId.HasValue && employees.ContainsKey(log.EmployeeId.Value)
                    ? employees[log.EmployeeId.Value]
                    : null,
                ActionType = ExtractActionType(log.Action),
                EntityType = ExtractEntityType(log.Action)
            }).ToList();

           
            var pdfBytes = _pdfService.GenerateAuditLogReport(viewModels, "Audit Log Report");

            var fileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }





        //Helper Methods
        private IQueryable<Models.Entities.AuditLog> ApplySorting(
            IQueryable<Models.Entities.AuditLog> query,
            string? sortBy,
            string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "timestamp";
                sortOrder = "desc";
            }

            var isDescending = sortOrder?.ToLower() == "desc";

            query = sortBy.ToLower() switch
            {
                "timestamp" => isDescending ? query.OrderByDescending(a => a.Timestamp) : query.OrderBy(a => a.Timestamp),
                "user" => isDescending ? query.OrderByDescending(a => a.UserId) : query.OrderBy(a => a.UserId),
                "action" => isDescending ? query.OrderByDescending(a => a.Action) : query.OrderBy(a => a.Action),
                _ => query.OrderByDescending(a => a.Timestamp)
            };

            return query;
        }

        private async Task PopulateDropdownsAsync(AuditLogSearchViewModel model)
        {
            // Get unique users
            var users = await _context.AuditLogs
                .Select(a => a.UserId)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();

            model.Users = users.Select(u => new SelectListItem
            {
                Value = u,
                Text = u
            }).ToList();

            model.Users.Insert(0, new SelectListItem { Value = "", Text = "All Users" });
        }

      
        private string ExtractActionType(string action)
        {
            if (action.Contains("Created", StringComparison.OrdinalIgnoreCase))
                return "Created";
            if (action.Contains("Updated", StringComparison.OrdinalIgnoreCase))
                return "Updated";
            if (action.Contains("Deleted", StringComparison.OrdinalIgnoreCase))
                return "Deleted";
            if (action.Contains("Deactivated", StringComparison.OrdinalIgnoreCase))
                return "Deactivated";

            return "Other";
        }

        
        private string ExtractEntityType(string action)
        {
            if (action.Contains("employee", StringComparison.OrdinalIgnoreCase))
                return "Employee";
            if (action.Contains("department", StringComparison.OrdinalIgnoreCase))
                return "Department";
            if (action.Contains("job title", StringComparison.OrdinalIgnoreCase))
                return "Job Title";

            return "System";
        }

        
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            
            return value.Replace("\"", "\"\"");
        }

       
    }
}