using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models.Entities
{
    public class UserSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string UserEmail { get; set; } = string.Empty;

        [StringLength(50)]
        public string? UserRole { get; set; }

       
        [Required]
        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public int? SessionDurationMinutes { get; set; }

      
        [StringLength(45)] // IPv4: xxx.xxx.xxx.xxx, IPv6: longer
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; } // Browser/Device info

        [StringLength(100)]
        public string? DeviceType { get; set; } // Desktop, Mobile, Tablet

        [StringLength(100)]
        public string? Browser { get; set; }

        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Ended, Expired, ForcedLogout

        // Security tracking
        public bool IsSuspicious { get; set; } = false;

        [StringLength(500)]
        public string? SuspiciousReason { get; set; }

        // Session identifier (for multi-device logout)
        [StringLength(100)]
        public string? SessionToken { get; set; }

    
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}