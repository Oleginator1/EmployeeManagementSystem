using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models.Entities
{

    public class Employee
    {

        [Key]
        public int EmployeeId { get; set; }


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
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; } 

       
        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

       
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        
        [Required(ErrorMessage = "Job title is required")]
        [Display(Name = "Job Title")]
        public int JobTitleId { get; set; }

       
        [ForeignKey("JobTitleId")]
        public JobTitle? JobTitle { get; set; }

        
        [Required(ErrorMessage = "Salary is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

       
        [Required(ErrorMessage = "Hire date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Now; 

        
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true; 

        
        [StringLength(255)]
        [Display(Name = "Profile Photo")]
        public string? ProfilePhotoPath { get; set; }

        
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(450)]
        [Display(Name = "User Account")]
        public string? UserId { get; set; }
    }
}