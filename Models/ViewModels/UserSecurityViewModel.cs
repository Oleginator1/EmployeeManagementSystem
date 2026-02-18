using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class UserSecurityViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;        

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Lockout End")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "Current Failed Attempts")]
        public int AccessFailedCount { get; set; }

        // NEW: Login statistics from audit log
        [Display(Name = "Total Failed Attempts (All Time)")]
        public int TotalFailedAttempts { get; set; }

        [Display(Name = "Total Successful Logins")]
        public int TotalSuccessfulLogins { get; set; }

        [Display(Name = "Last Successful Login")]
        public DateTime? LastSuccessfulLogin { get; set; }

        [Display(Name = "Last Failed Login")]
        public DateTime? LastFailedLogin { get; set; }

        
        public List<UserActivityViewModel> RecentActivity { get; set; } = new();

       
        public List<UserActivityViewModel> LoginHistory { get; set; } = new();
    }

    public class UserActivityViewModel
    {
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}