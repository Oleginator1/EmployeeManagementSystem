using EmployeeManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                
                entity.HasIndex(e => e.Email)
                    .IsUnique();

               
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);  

           
                entity.HasOne(e => e.JobTitle)
                    .WithMany(j => j.Employees)
                    .HasForeignKey(e => e.JobTitleId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "Information Technology" },
                new Department { DepartmentId = 2, Name = "Human Resources" },
                new Department { DepartmentId = 3, Name = "Finance" },
                new Department { DepartmentId = 4, Name = "Sales" },
                new Department { DepartmentId = 5, Name = "Marketing" }
            );

            modelBuilder.Entity<JobTitle>().HasData(
                new JobTitle { JobTitleId = 1, TitleName = "Software Developer" },
                new JobTitle { JobTitleId = 2, TitleName = "Senior Developer" },
                new JobTitle { JobTitleId = 3, TitleName = "Project Manager" },
                new JobTitle { JobTitleId = 4, TitleName = "HR Manager" },
                new JobTitle { JobTitleId = 5, TitleName = "Accountant" },
                new JobTitle { JobTitleId = 6, TitleName = "Sales Representative" },
                new JobTitle { JobTitleId = 7, TitleName = "Marketing Specialist" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@company.com",
                    Phone = "+1234567890",
                    DepartmentId = 1, 
                    JobTitleId = 2,    
                    Salary = 75000.00m,
                    HireDate = new DateTime(2023, 1, 15),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
