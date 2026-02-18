using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.ViewModels
{

    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;


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