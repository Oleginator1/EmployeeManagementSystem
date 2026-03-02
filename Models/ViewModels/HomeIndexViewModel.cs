namespace EmployeeManagementSystem.Models.ViewModels
{

    public class HomeIndexViewModel
    {
        // User information
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? UserRole { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public bool IsEmployee { get; set; }

        // Statistics (for Admin/Manager)
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalJobTitles { get; set; }
        public decimal AverageSalary { get; set; }
        public int NewEmployeesThisMonth { get; set; }


        public List<RecentActivityItem> RecentActivities { get; set; } = new();


        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public List<UpcomingBirthday> UpcomingBirthdays { get; set; } = new();

        // Employee-specific data
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? HireDate { get; set; }
        public int? YearsOfService { get; set; }
    }

    public class RecentActivityItem
    {
        public string Action { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = "bi-info-circle";
        public string BadgeColor { get; set; } = "secondary";
    }

    public class UpcomingBirthday
    {
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int DaysUntil { get; set; }
    }
}