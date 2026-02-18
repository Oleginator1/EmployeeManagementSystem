using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class EmployeeSearchViewModel
    {
        // Search and Filter Parameters
        public string? SearchTerm { get; set; }
        public int? DepartmentFilter { get; set; }
        public int? JobTitleFilter { get; set; }
        public string? StatusFilter { get; set; } 
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }

        // Sorting Parameters
        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; } 

        // Pagination Parameters
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // Results
        public List<EmployeeViewModel> Employees { get; set; } = new();

        // Dropdown Options
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> JobTitles { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new()
        {
            new SelectListItem { Value = "", Text = "All Status" },
            new SelectListItem { Value = "active", Text = "Active Only" },
            new SelectListItem { Value = "inactive", Text = "Inactive Only" }
        };

        // Statistics
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }

        // Helper Properties
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasFilters => !string.IsNullOrEmpty(SearchTerm)
            || DepartmentFilter.HasValue
            || JobTitleFilter.HasValue
            || !string.IsNullOrEmpty(StatusFilter)
            || MinSalary.HasValue
            || MaxSalary.HasValue
            || HireDateFrom.HasValue
            || HireDateTo.HasValue;
    }
}
