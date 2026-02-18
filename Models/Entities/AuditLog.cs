using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.Entities
{

    public class AuditLog
    {

        [Key]
        public int Id { get; set; }

 
        [Required]
        [StringLength(200)]
        public string Action { get; set; } = string.Empty;

        
        [Required]
        [StringLength(256)]
        public string UserId { get; set; } = string.Empty;

        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

       
        [StringLength(500)]
        public string? Details { get; set; }

        
        public int? EmployeeId { get; set; }
    }
}