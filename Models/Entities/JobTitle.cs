using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models.Entities
{

    public class JobTitle
    {
        [Key]
        public int JobTitleId { get; set; }

        [Required(ErrorMessage = "Job title is required")]
        [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters")]
        public string TitleName { get; set; } = string.Empty;

        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxSalary { get; set; }

       
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}