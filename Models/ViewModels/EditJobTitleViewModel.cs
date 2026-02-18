using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{

    public class EditJobTitleViewModel
    {
        public int JobTitleId { get; set; }

        [Required(ErrorMessage = "Job title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Job title must be between 2 and 100 characters")]
        [Display(Name = "Job Title")]
        public string TitleName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Salary Range (Min)")]
        [DataType(DataType.Currency)]
        [Range(0, 9999999.99, ErrorMessage = "Minimum salary must be between 0 and 9,999,999.99")]
        public decimal? MinSalary { get; set; }

        [Display(Name = "Salary Range (Max)")]
        [DataType(DataType.Currency)]
        [Range(0, 9999999.99, ErrorMessage = "Maximum salary must be between 0 and 9,999,999.99")]
        public decimal? MaxSalary { get; set; }

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }
    }
}