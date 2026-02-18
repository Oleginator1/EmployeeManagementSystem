using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    
    public class JobTitleViewModel
    {
        public int JobTitleId { get; set; }

        [Display(Name = "Job Title")]
        public string TitleName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }

        [Display(Name = "Active Employees")]
        public int ActiveEmployeeCount { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}