using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.ViewModels;

namespace EmployeeManagementSystem.Services
{
   
    public interface IPdfService
    {
        
        byte[] GenerateEmployeeListReport(List<EmployeeViewModel> employees, string title = "Employee Report");

        
        byte[] GenerateEmployeeDetailsReport(Employee employee);

        
        byte[] GenerateDepartmentReport(List<Department> departments);

       
        byte[] GenerateJobTitleReport(List<JobTitle> jobTitles);

      
        byte[] GenerateAuditLogReport(List<AuditLogViewModel> logs, string title = "Audit Log Report");
    }
}