using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Models.ViewModels
{

    public class AuditLogSearchViewModel
    {
        // Search and Filter Parameters
        public string? SearchTerm { get; set; }
        public string? UserFilter { get; set; }
        public string? ActionTypeFilter { get; set; }
        public string? EntityTypeFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // Sorting Parameters
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

        // Pagination Parameters
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // Results
        public List<AuditLogViewModel> AuditLogs { get; set; } = new();

        // Dropdown Options
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> ActionTypes { get; set; } = new()
        {
            new SelectListItem { Value = "", Text = "All Actions" },
            new SelectListItem { Value = "Created", Text = "Created" },
            new SelectListItem { Value = "Updated", Text = "Updated" },
            new SelectListItem { Value = "Deleted", Text = "Deleted" },
            new SelectListItem { Value = "Deactivated", Text = "Deactivated" }
        };
        public List<SelectListItem> EntityTypes { get; set; } = new()
        {
            new SelectListItem { Value = "", Text = "All Entities" },
            new SelectListItem { Value = "Employee", Text = "Employee" },
            new SelectListItem { Value = "Department", Text = "Department" },
            new SelectListItem { Value = "Job Title", Text = "Job Title" }
        };

        // Statistics
        public int TotalActions { get; set; }
        public int ActionsToday { get; set; }
        public int ActionsThisWeek { get; set; }

        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasFilters => !string.IsNullOrEmpty(SearchTerm)
            || !string.IsNullOrEmpty(UserFilter)
            || !string.IsNullOrEmpty(ActionTypeFilter)
            || !string.IsNullOrEmpty(EntityTypeFilter)
            || DateFrom.HasValue
            || DateTo.HasValue;
    }
}