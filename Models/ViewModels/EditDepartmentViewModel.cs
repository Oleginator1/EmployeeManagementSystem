using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class EditDepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }
    }
}