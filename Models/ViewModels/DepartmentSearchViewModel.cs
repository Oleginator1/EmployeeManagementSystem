using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class DepartmentSearchViewModel
    {
        // Search parameters
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // Results
        public List<DepartmentViewModel> Departments { get; set; } = new();

        // Statistics
        public int TotalDepartments { get; set; }
        public int TotalEmployees { get; set; }

       
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}