using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a job title")]
        [Display(Name = "Job Title")]
        public int JobTitleId { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0.01, 9999999.99, ErrorMessage = "Salary must be between 0.01 and 9,999,999.99")]
        [Display(Name = "Salary")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Hire date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfilePhoto { get; set; }

        // Dropdown lists - populated by controller
        // SelectListItem is a class that represents an option in a dropdown
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> JobTitles { get; set; } = new();
    }
}
