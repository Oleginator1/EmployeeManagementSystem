using EmployeeManagementSystem.Constants;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    
    [Authorize(Policy = "AdminOnly")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IPdfService _pdfService;

        public DepartmentsController(ApplicationDbContext context, IUserService userService, IPdfService pdfService)
        {
            _context = context;
            _userService = userService;
            _pdfService = pdfService;
        }


        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, string? sortOrder)
        {
           
            var query = _context.Departments
                .Include(d => d.Employees)
                .AsQueryable();

         
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search) ||
                    (d.Description != null && d.Description.ToLower().Contains(search))
                );
            }

            
            sortBy = sortBy?.ToLower() ?? "name";
            sortOrder = sortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "name" => sortOrder == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "employeecount" => sortOrder == "desc" ? query.OrderByDescending(d => d.Employees.Count) : query.OrderBy(d => d.Employees.Count),
                "created" => sortOrder == "desc" ? query.OrderByDescending(d => d.CreatedDate) : query.OrderBy(d => d.CreatedDate),
                _ => query.OrderBy(d => d.Name)
            };

            var departments = await query.ToListAsync();

            
            var viewModel = departments.Select(d => new DepartmentViewModel
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description,  
                EmployeeCount = d.Employees.Count,
                ActiveEmployeeCount = d.Employees.Count(e => e.IsActive),
                CreatedDate = d.CreatedDate
            }).ToList();

            
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SortBy"] = sortBy;
            ViewData["SortOrder"] = sortOrder;
            ViewData["NextSortOrder"] = sortOrder == "asc" ? "desc" : "asc";

            return View(viewModel);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Employees)
                    .ThenInclude(e => e.JobTitle)
                .FirstOrDefaultAsync(m => m.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

       
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
               
                var exists = await _context.Departments
                    .AnyAsync(d => d.Name.ToLower() == viewModel.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "A department with this name already exists.");
                    return View(viewModel);
                }

                
                var department = new Department
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    CreatedDate = DateTime.Now
                };

                _context.Add(department);
                await _context.SaveChangesAsync();

                
                await LogActionAsync($"Created department: {department.Name}");

                TempData["SuccessMessage"] = $"Department '{department.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

            var viewModel = new EditDepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = department.Employees.Count
            };

            return View(viewModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDepartmentViewModel viewModel)
        {
            if (id != viewModel.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var department = await _context.Departments.FindAsync(id);

                    if (department == null)
                    {
                        return NotFound();
                    }


                    if (department.Name != viewModel.Name)
                    {
                        var exists = await _context.Departments
                            .AnyAsync(d => d.Name.ToLower() == viewModel.Name.ToLower() && d.DepartmentId != id);

                        if (exists)
                        {
                            ModelState.AddModelError("Name", "A department with this name already exists.");
                            return View(viewModel);
                        }
                    }

                   
                    department.Name = viewModel.Name;
                    department.Description = viewModel.Description;
                    department.ModifiedDate = DateTime.Now;

                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    
                    await LogActionAsync($"Updated department: {department.Name}");

                    TempData["SuccessMessage"] = $"Department '{department.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartmentExistsAsync(viewModel.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(viewModel);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(m => m.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

           
            if (department.Employees.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete department '{department.Name}' because it has {department.Employees.Count} employee(s) assigned to it. Please reassign or remove employees first.";
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }

           
            if (department.Employees.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete department '{department.Name}' because it has employees assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            var departmentName = department.Name;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            
            await LogActionAsync($"Deleted department: {departmentName}");

            TempData["SuccessMessage"] = $"Department '{departmentName}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ExportPdf(string? searchTerm, string? sortBy, string? sortOrder)
        {
            var query = _context.Departments
                .Include(d => d.Employees)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search) ||
                    (d.Description != null && d.Description.ToLower().Contains(search))
                );
            }

            
            sortBy = sortBy?.ToLower() ?? "name";
            sortOrder = sortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "name" => sortOrder == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "employeecount" => sortOrder == "desc" ? query.OrderByDescending(d => d.Employees.Count) : query.OrderBy(d => d.Employees.Count),
                _ => query.OrderBy(d => d.Name)
            };

            var departments = await query.ToListAsync();

            
            var pdfBytes = _pdfService.GenerateDepartmentReport(departments);

           
            await LogActionAsync($"Exported {departments.Count} departments to PDF");

            var fileName = $"Departments_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }





        //Helper Methods
        private async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(e => e.DepartmentId == id);
        }

        
        private async Task LogActionAsync(string action)
        {
            var log = new AuditLog
            {
                Action = action,
                UserId = _userService.GetCurrentUserEmail() ?? "System",
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        
    }
}