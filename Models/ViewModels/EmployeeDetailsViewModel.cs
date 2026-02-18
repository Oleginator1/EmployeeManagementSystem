using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public int EmployeeId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
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

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Years of Service")]
        public int YearsOfService { get; set; }
    }
}
