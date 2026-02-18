using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class UserListViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new();

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Locked Out")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Total Failed Logins")]
        public int AccessFailedCount { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }      

        [Display(Name = "Linked Employee")]
        public int? LinkedEmployeeId { get; set; }

        [Display(Name = "Employee Name")]
        public string? LinkedEmployeeName { get; set; }
    }
}