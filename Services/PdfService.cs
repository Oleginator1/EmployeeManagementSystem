using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.ViewModels;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace EmployeeManagementSystem.Services
{

    public class PdfService : IPdfService
    {

        public byte[] GenerateEmployeeListReport(List<EmployeeViewModel> employees, string title = "Employee Report")
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            
            AddHeader(document, title);
            AddReportInfo(document, $"Total Employees: {employees.Count}");

            // Add summary statistics
            var activeCount = employees.Count(e => e.IsActive);
            var inactiveCount = employees.Count - activeCount;

            var summaryTable = new Table(3).UseAllAvailableWidth();
            summaryTable.AddHeaderCell(CreateHeaderCell("Total"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Active"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Inactive"));
            summaryTable.AddCell(CreateCell(employees.Count.ToString()));
            summaryTable.AddCell(CreateCell(activeCount.ToString()));
            summaryTable.AddCell(CreateCell(inactiveCount.ToString()));
            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));

            // Add employee table
            var table = new Table(new float[] { 3, 2, 2, 2, 1.5f, 1.5f })
                .UseAllAvailableWidth();

            
            table.AddHeaderCell(CreateHeaderCell("Name"));
            table.AddHeaderCell(CreateHeaderCell("Email"));
            table.AddHeaderCell(CreateHeaderCell("Department"));
            table.AddHeaderCell(CreateHeaderCell("Job Title"));
            table.AddHeaderCell(CreateHeaderCell("Salary"));
            table.AddHeaderCell(CreateHeaderCell("Status"));

           
            foreach (var employee in employees)
            {
                table.AddCell(CreateCell(employee.FullName));
                table.AddCell(CreateCell(employee.Email));
                table.AddCell(CreateCell(employee.DepartmentName));
                table.AddCell(CreateCell(employee.JobTitleName));
                table.AddCell(CreateCell(employee.Salary.ToString("C")));
                table.AddCell(CreateCell(employee.Status));
            }

            document.Add(table);

           
            AddFooter(document);

            document.Close();
            return memoryStream.ToArray();
        }

      
        public byte[] GenerateEmployeeDetailsReport(Employee employee)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            
            AddHeader(document, "Employee Details Report");
            document.Add(new Paragraph($"Employee: {employee.FullName}")
                .SetFontSize(16)
                .SetBold()
                .SetMarginBottom(20));

            // Personal Information Section
            document.Add(new Paragraph("Personal Information")
                .SetFontSize(14)
                .SetBold()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(5));

            var personalTable = new Table(2).UseAllAvailableWidth();
            personalTable.AddCell(CreateLabelCell("First Name:"));
            personalTable.AddCell(CreateCell(employee.FirstName));
            personalTable.AddCell(CreateLabelCell("Last Name:"));
            personalTable.AddCell(CreateCell(employee.LastName));
            personalTable.AddCell(CreateLabelCell("Email:"));
            personalTable.AddCell(CreateCell(employee.Email));
            personalTable.AddCell(CreateLabelCell("Phone:"));
            personalTable.AddCell(CreateCell(employee.Phone ?? "N/A"));
            document.Add(personalTable);
            document.Add(new Paragraph("\n"));

            // Employment Information Section
            document.Add(new Paragraph("Employment Information")
                .SetFontSize(14)
                .SetBold()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(5));

            var employmentTable = new Table(2).UseAllAvailableWidth();
            employmentTable.AddCell(CreateLabelCell("Department:"));
            employmentTable.AddCell(CreateCell(employee.Department?.Name ?? "N/A"));
            employmentTable.AddCell(CreateLabelCell("Job Title:"));
            employmentTable.AddCell(CreateCell(employee.JobTitle?.TitleName ?? "N/A"));
            employmentTable.AddCell(CreateLabelCell("Salary:"));
            employmentTable.AddCell(CreateCell(employee.Salary.ToString("C")));
            employmentTable.AddCell(CreateLabelCell("Hire Date:"));
            employmentTable.AddCell(CreateCell(employee.HireDate.ToString("MMMM dd, yyyy")));
            employmentTable.AddCell(CreateLabelCell("Status:"));
            employmentTable.AddCell(CreateCell(employee.IsActive ? "Active" : "Inactive"));
            document.Add(employmentTable);
            document.Add(new Paragraph("\n"));

            // Audit Information Section
            document.Add(new Paragraph("Record Information")
                .SetFontSize(14)
                .SetBold()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(5));

            var auditTable = new Table(2).UseAllAvailableWidth();
            auditTable.AddCell(CreateLabelCell("Created Date:"));
            auditTable.AddCell(CreateCell(employee.CreatedDate.ToString("MMMM dd, yyyy hh:mm tt")));
            if (employee.ModifiedDate.HasValue)
            {
                auditTable.AddCell(CreateLabelCell("Last Modified:"));
                auditTable.AddCell(CreateCell(employee.ModifiedDate.Value.ToString("MMMM dd, yyyy hh:mm tt")));
            }
            document.Add(auditTable);

            
            AddFooter(document);

            document.Close();
            return memoryStream.ToArray();
        }

        
        public byte[] GenerateDepartmentReport(List<Department> departments)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

         
            AddHeader(document, "Departments Report");
            AddReportInfo(document, $"Total Departments: {departments.Count}");

            // Add summary
            var totalEmployees = departments.Sum(d => d.Employees.Count);
            var summaryTable = new Table(2).UseAllAvailableWidth();
            summaryTable.AddHeaderCell(CreateHeaderCell("Total Departments"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Total Employees"));
            summaryTable.AddCell(CreateCell(departments.Count.ToString()));
            summaryTable.AddCell(CreateCell(totalEmployees.ToString()));
            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));

            // Add department details table
            var table = new Table(new float[] { 3, 4, 2, 2 }).UseAllAvailableWidth();

            table.AddHeaderCell(CreateHeaderCell("Department Name"));
            table.AddHeaderCell(CreateHeaderCell("Description"));
            table.AddHeaderCell(CreateHeaderCell("Employees"));
            table.AddHeaderCell(CreateHeaderCell("Active"));

            foreach (var dept in departments.OrderBy(d => d.Name))
            {
                table.AddCell(CreateCell(dept.Name));
                table.AddCell(CreateCell(dept.Description ?? "No description"));
                table.AddCell(CreateCell(dept.Employees.Count.ToString()));
                table.AddCell(CreateCell(dept.Employees.Count(e => e.IsActive).ToString()));
            }

            document.Add(table);

            
            AddFooter(document);

            document.Close();
            return memoryStream.ToArray();
        }



        public byte[] GenerateJobTitleReport(List<JobTitle> jobTitles)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

           
            AddHeader(document, "Job Titles Report");
            AddReportInfo(document, $"Total Job Titles: {jobTitles.Count}");

            // Add summary
            var totalEmployees = jobTitles.Sum(j => j.Employees.Count);
            var summaryTable = new Table(2).UseAllAvailableWidth();
            summaryTable.AddHeaderCell(CreateHeaderCell("Total Job Titles"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Total Employees"));
            summaryTable.AddCell(CreateCell(jobTitles.Count.ToString()));
            summaryTable.AddCell(CreateCell(totalEmployees.ToString()));
            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));

            // Add job title details table
            var table = new Table(new float[] { 3, 4, 2, 2, 2 }).UseAllAvailableWidth();

            table.AddHeaderCell(CreateHeaderCell("Job Title"));
            table.AddHeaderCell(CreateHeaderCell("Description"));
            table.AddHeaderCell(CreateHeaderCell("Salary Range"));
            table.AddHeaderCell(CreateHeaderCell("Employees"));
            table.AddHeaderCell(CreateHeaderCell("Active"));

            foreach (var job in jobTitles.OrderBy(j => j.TitleName))
            {
                table.AddCell(CreateCell(job.TitleName));
                table.AddCell(CreateCell(job.Description ?? "No description"));

                string salaryRange = "Not specified";
                if (job.MinSalary.HasValue && job.MaxSalary.HasValue)
                {
                    salaryRange = $"{job.MinSalary.Value:C} - {job.MaxSalary.Value:C}";
                }
                else if (job.MinSalary.HasValue)
                {
                    salaryRange = $"From {job.MinSalary.Value:C}";
                }
                else if (job.MaxSalary.HasValue)
                {
                    salaryRange = $"Up to {job.MaxSalary.Value:C}";
                }

                table.AddCell(CreateCell(salaryRange));
                table.AddCell(CreateCell(job.Employees.Count.ToString()));
                table.AddCell(CreateCell(job.Employees.Count(e => e.IsActive).ToString()));
            }

            document.Add(table);

       
            AddFooter(document);

            document.Close();
            return memoryStream.ToArray();
        }

       
        public byte[] GenerateAuditLogReport(List<AuditLogViewModel> logs, string title = "Audit Log Report")
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

          
            AddHeader(document, title);
            AddReportInfo(document, $"Total Log Entries: {logs.Count}");

            // Add summary statistics
            var createdCount = logs.Count(l => l.ActionType == "Created");
            var updatedCount = logs.Count(l => l.ActionType == "Updated");
            var deletedCount = logs.Count(l => l.ActionType == "Deleted");

            var summaryTable = new Table(4).UseAllAvailableWidth();
            summaryTable.AddHeaderCell(CreateHeaderCell("Total"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Created"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Updated"));
            summaryTable.AddHeaderCell(CreateHeaderCell("Deleted"));
            summaryTable.AddCell(CreateCell(logs.Count.ToString()));
            summaryTable.AddCell(CreateCell(createdCount.ToString()));
            summaryTable.AddCell(CreateCell(updatedCount.ToString()));
            summaryTable.AddCell(CreateCell(deletedCount.ToString()));
            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));

            // Add audit log table
            var table = new Table(new float[] { 1.5f, 3, 2, 2, 2 }).UseAllAvailableWidth();

            table.AddHeaderCell(CreateHeaderCell("Timestamp"));
            table.AddHeaderCell(CreateHeaderCell("User"));
            table.AddHeaderCell(CreateHeaderCell("Action"));
            table.AddHeaderCell(CreateHeaderCell("Type"));
            table.AddHeaderCell(CreateHeaderCell("Entity"));

            foreach (var log in logs)
            {
                table.AddCell(CreateCell(log.Timestamp.ToString("MM/dd/yyyy HH:mm")));
                table.AddCell(CreateCell(log.UserId));
                table.AddCell(CreateCell(log.Action));
                table.AddCell(CreateCell(log.ActionType));
                table.AddCell(CreateCell(log.EntityType));
            }

            document.Add(table);

           
            AddFooter(document);

            document.Close();
            return memoryStream.ToArray();
        }

        

      
        
        private void AddHeader(Document document, string title)
        {
            var header = new Paragraph(title)
                .SetFontSize(20)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10);

            document.Add(header);

            var companyInfo = new Paragraph("Employee Management System")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);

            document.Add(companyInfo);
        }

 
        private void AddReportInfo(Document document, string info)
        {
            var reportInfo = new Paragraph($"{info}\nGenerated on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}")
                .SetFontSize(10)
                .SetItalic()
                .SetMarginBottom(15);

            document.Add(reportInfo);
        }

   
        private void AddFooter(Document document)
        {
            document.Add(new Paragraph("\n"));

            var footer = new Paragraph($"Page generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.GRAY);

            document.Add(footer);
        }

        
        private Cell CreateHeaderCell(string text)
        {
            return new Cell()
                .Add(new Paragraph(text).SetBold())
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(5);
        }

       
        private Cell CreateCell(string text)
        {
            return new Cell()
                .Add(new Paragraph(text))
                .SetPadding(5);
        }

       
        private Cell CreateLabelCell(string text)
        {
            return new Cell()
                .Add(new Paragraph(text).SetBold())
                .SetBackgroundColor(new DeviceRgb(240, 240, 240))
                .SetPadding(5);
        }

       
    }
}