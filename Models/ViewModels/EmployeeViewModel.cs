using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        [Display(Name = "Job Title")]
        public string JobTitleName { get; set; } = string.Empty;

        [Display(Name = "Salary")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty; 

        public bool IsActive { get; set; }

        [Display(Name = "Profile Photo")]
        public string? ProfilePhotoPath { get; set; }
    }
}
