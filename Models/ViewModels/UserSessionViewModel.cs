using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class UserSessionViewModel
    {
        public int SessionId { get; set; }

        [Display(Name = "User")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string? UserRole { get; set; }

        [Display(Name = "Login Time")]
        public DateTime LoginTime { get; set; }

        [Display(Name = "Logout Time")]
        public DateTime? LogoutTime { get; set; }

        [Display(Name = "Duration")]
        public string Duration { get; set; } = string.Empty;

        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        [Display(Name = "Device")]
        public string? DeviceType { get; set; }

        [Display(Name = "Browser")]
        public string? Browser { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        public bool IsActive => Status == "Active";
        public bool IsSuspicious { get; set; }
    }

    public class SessionManagementViewModel
    {
        
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

       
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

       
        public List<UserSessionViewModel> Sessions { get; set; } = new();

      
        public int ActiveSessions { get; set; }
        public int TotalSessionsToday { get; set; }
        public int UniqueUsersToday { get; set; }
        public double AverageSessionMinutes { get; set; }

        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}