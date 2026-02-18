using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{

    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Account Created")]
        public DateTime? CreatedDate { get; set; }

        
        public List<string> Roles { get; set; } = new();

        
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
    }
}