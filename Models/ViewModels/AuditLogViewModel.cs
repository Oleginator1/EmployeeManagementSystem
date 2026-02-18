using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
   
    public class AuditLogViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; } = string.Empty;

        [Display(Name = "User")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Details")]
        public string? Details { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }

        
        [Display(Name = "Action Type")]
        public string ActionType { get; set; } = string.Empty; 

        [Display(Name = "Entity Type")]
        public string EntityType { get; set; } = string.Empty; 
    }
}