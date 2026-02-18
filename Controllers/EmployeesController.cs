using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfService _pdfService;

        public EmployeesController(
           ApplicationDbContext context,
           IUserService userService,
           IWebHostEnvironment webHostEnvironment,
           IPdfService pdfService)
        {
            _context = context;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _pdfService = pdfService;
        }

        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Index(EmployeeSearchViewModel searchModel)
        {
            
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm)
                );
            }

            
            if (searchModel.DepartmentFilter.HasValue)
            {
                query = query.Where(e => e.DepartmentId == searchModel.DepartmentFilter.Value);
            }

            
            if (searchModel.JobTitleFilter.HasValue)
            {
                query = query.Where(e => e.JobTitleId == searchModel.JobTitleFilter.Value);
            }

           
            if (!string.IsNullOrEmpty(searchModel.StatusFilter))
            {
                if (searchModel.StatusFilter.ToLower() == "active")
                {
                    query = query.Where(e => e.IsActive);
                }
                else if (searchModel.StatusFilter.ToLower() == "inactive")
                {
                    query = query.Where(e => !e.IsActive);
                }
            }

           
            if (searchModel.MinSalary.HasValue)
            {
                query = query.Where(e => e.Salary >= searchModel.MinSalary.Value);
            }
            if (searchModel.MaxSalary.HasValue)
            {
                query = query.Where(e => e.Salary <= searchModel.MaxSalary.Value);
            }

           
            if (searchModel.HireDateFrom.HasValue)
            {
                query = query.Where(e => e.HireDate >= searchModel.HireDateFrom.Value);
            }
            if (searchModel.HireDateTo.HasValue)
            {
                query = query.Where(e => e.HireDate <= searchModel.HireDateTo.Value);
            }

           
            searchModel.TotalRecords = await query.CountAsync();

            
            query = ApplySorting(query, searchModel.SortBy, searchModel.SortOrder);

           
            var employees = await query
                .Skip((searchModel.PageNumber - 1) * searchModel.PageSize)
                .Take(searchModel.PageSize)
                .ToListAsync();

            
            searchModel.Employees = employees.Select(e => new EmployeeViewModel
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentName = e.Department?.Name ?? "N/A",
                JobTitleName = e.JobTitle?.TitleName ?? "N/A",
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive,
                Status = e.IsActive ? "Active" : "Inactive",
                ProfilePhotoPath = e.ProfilePhotoPath
            }).ToList();

            
            await PopulateSearchDropdownsAsync(searchModel);

            // Calculate statistics
            var allEmployees = await _context.Employees.ToListAsync();
            searchModel.TotalEmployees = allEmployees.Count;
            searchModel.ActiveEmployees = allEmployees.Count(e => e.IsActive);
            searchModel.InactiveEmployees = allEmployees.Count(e => !e.IsActive);

            return View(searchModel);
        }

        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            var yearsOfService = DateTime.Now.Year - employee.HireDate.Year;
            if (DateTime.Now.DayOfYear < employee.HireDate.DayOfYear)
            {
                yearsOfService--; 
            }

            var viewModel = new EmployeeDetailsViewModel
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentName = employee.Department?.Name ?? "N/A",
                JobTitleName = employee.JobTitle?.TitleName ?? "N/A",
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                Status = employee.IsActive ? "Active" : "Inactive",
                ProfilePhotoPath = employee.ProfilePhotoPath,
                CreatedDate = employee.CreatedDate,
                ModifiedDate = employee.ModifiedDate,
                YearsOfService = yearsOfService
            };

            return View(viewModel);
        }

        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateEmployeeViewModel();

            // Populate dropdown lists
            await PopulateDropdownListsAsync(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Create(CreateEmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                
                var emailExists = await _context.Employees
                    .AnyAsync(e => e.Email == viewModel.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "An employee with this email already exists.");
                    await PopulateDropdownListsAsync(viewModel);
                    return View(viewModel);
                }

                
                string? photoPath = null;
                if (viewModel.ProfilePhoto != null)
                {
                    photoPath = await SaveProfilePhotoAsync(viewModel.ProfilePhoto);
                }

               
                var employee = new Employee
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    Phone = viewModel.Phone,
                    DepartmentId = viewModel.DepartmentId,
                    JobTitleId = viewModel.JobTitleId,
                    Salary = viewModel.Salary,
                    HireDate = viewModel.HireDate,
                    IsActive = true,
                    ProfilePhotoPath = photoPath,
                    CreatedDate = DateTime.Now
                };

                _context.Add(employee);
                await _context.SaveChangesAsync();

                
                await LogActionAsync($"Created employee: {employee.FullName}", employee.EmployeeId);

                
                TempData["SuccessMessage"] = $"Employee {employee.FullName} created successfully!";

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownListsAsync(viewModel);
            return View(viewModel);
        }



        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

           
            var viewModel = new EditEmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentId = employee.DepartmentId,
                JobTitleId = employee.JobTitleId,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                CurrentProfilePhotoPath = employee.ProfilePhotoPath
            };

            await PopulateDropdownListsAsync(viewModel);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel viewModel)
        {
            if (id != viewModel.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _context.Employees.FindAsync(id);
                    if (employee == null)
                    {
                        return NotFound();
                    }

                    if (employee.Email != viewModel.Email)
                    {
                        var emailExists = await _context.Employees
                            .AnyAsync(e => e.Email == viewModel.Email && e.EmployeeId != id);

                        if (emailExists)
                        {
                            ModelState.AddModelError("Email", "An employee with this email already exists.");
                            await PopulateDropdownListsAsync(viewModel);
                            return View(viewModel);
                        }
                    }

                  
                    if (viewModel.ProfilePhoto != null)
                    {
                      
                        if (!string.IsNullOrEmpty(employee.ProfilePhotoPath))
                        {
                            DeleteProfilePhoto(employee.ProfilePhotoPath);
                        }

                        employee.ProfilePhotoPath = await SaveProfilePhotoAsync(viewModel.ProfilePhoto);
                    }


                    employee.FirstName = viewModel.FirstName;
                    employee.LastName = viewModel.LastName;
                    employee.Email = viewModel.Email;
                    employee.Phone = viewModel.Phone;
                    employee.DepartmentId = viewModel.DepartmentId;
                    employee.JobTitleId = viewModel.JobTitleId;
                    employee.Salary = viewModel.Salary;
                    employee.HireDate = viewModel.HireDate;
                    employee.IsActive = viewModel.IsActive;
                    employee.ModifiedDate = DateTime.Now;

                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                   
                    await LogActionAsync($"Updated employee: {employee.FullName}", employee.EmployeeId);

                    TempData["SuccessMessage"] = $"Employee {employee.FullName} updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExistsAsync(viewModel.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateDropdownListsAsync(viewModel);
            return View(viewModel);
        }



        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.IsActive = false;
                employee.ModifiedDate = DateTime.Now;

                _context.Update(employee);
                await _context.SaveChangesAsync();

                await LogActionAsync($"Deactivated employee: {employee.FullName}", employee.EmployeeId);

                TempData["SuccessMessage"] = $"Employee {employee.FullName} has been deactivated.";
            }

            return RedirectToAction(nameof(Index));
        }



        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> ExportPdf(EmployeeSearchViewModel searchModel)
        {
            // Build same query as Index
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm)
                );
            }

            if (searchModel.DepartmentFilter.HasValue)
            {
                query = query.Where(e => e.DepartmentId == searchModel.DepartmentFilter.Value);
            }

            if (searchModel.JobTitleFilter.HasValue)
            {
                query = query.Where(e => e.JobTitleId == searchModel.JobTitleFilter.Value);
            }

            if (!string.IsNullOrEmpty(searchModel.StatusFilter))
            {
                if (searchModel.StatusFilter.ToLower() == "active")
                {
                    query = query.Where(e => e.IsActive);
                }
                else if (searchModel.StatusFilter.ToLower() == "inactive")
                {
                    query = query.Where(e => !e.IsActive);
                }
            }

            if (searchModel.MinSalary.HasValue)
            {
                query = query.Where(e => e.Salary >= searchModel.MinSalary.Value);
            }

            if (searchModel.MaxSalary.HasValue)
            {
                query = query.Where(e => e.Salary <= searchModel.MaxSalary.Value);
            }

            if (searchModel.HireDateFrom.HasValue)
            {
                query = query.Where(e => e.HireDate >= searchModel.HireDateFrom.Value);
            }

            if (searchModel.HireDateTo.HasValue)
            {
                query = query.Where(e => e.HireDate <= searchModel.HireDateTo.Value);
            }

            
            query = ApplySorting(query, searchModel.SortBy, searchModel.SortOrder);

            var employees = await query.ToListAsync();

            // Map to view models
            var viewModels = employees.Select(e => new EmployeeViewModel
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentName = e.Department?.Name ?? "N/A",
                JobTitleName = e.JobTitle?.TitleName ?? "N/A",
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive,
                Status = e.IsActive ? "Active" : "Inactive"
            }).ToList();

            
            var pdfBytes = _pdfService.GenerateEmployeeListReport(viewModels, "Employee Report");

            
            await LogActionAsync($"Exported {employees.Count} employees to PDF");

            var fileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        


        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> ExportDetailsPdf(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            
            var pdfBytes = _pdfService.GenerateEmployeeDetailsReport(employee);

            
            await LogActionAsync($"Exported employee details to PDF: {employee.FullName}", employee.EmployeeId);

            var fileName = $"Employee_{employee.LastName}_{employee.FirstName}_{DateTime.Now:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }



        //Helper Methods
        private async Task PopulateDropdownListsAsync(dynamic viewModel)
        {
           
            var departments = await _context.Departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                })
                .ToListAsync();

            
            var jobTitles = await _context.JobTitles
                .OrderBy(j => j.TitleName)
                .Select(j => new SelectListItem
                {
                    Value = j.JobTitleId.ToString(),
                    Text = j.TitleName
                })
                .ToListAsync();

            viewModel.Departments = departments;
            viewModel.JobTitles = jobTitles;
        }



        private async Task PopulateSearchDropdownsAsync(EmployeeSearchViewModel model)
        {
            // Get departments
            model.Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                })
                .ToListAsync();

            
            model.Departments.Insert(0, new SelectListItem { Value = "", Text = "All Departments" });

            // Get job titles
            model.JobTitles = await _context.JobTitles
                .OrderBy(j => j.TitleName)
                .Select(j => new SelectListItem
                {
                    Value = j.JobTitleId.ToString(),
                    Text = j.TitleName
                })
                .ToListAsync();

            
            model.JobTitles.Insert(0, new SelectListItem { Value = "", Text = "All Job Titles" });
        }




        private async Task<string> SaveProfilePhotoAsync(IFormFile photo)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");

            
            Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

           
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            
            return "/images/profiles/" + uniqueFileName;
        }

        private void DeleteProfilePhoto(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                return;

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, photoPath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }



        private async Task LogActionAsync(string action, int? employeeId = null)
        {
            var log = new AuditLog
            {
                Action = action,
                UserId = _userService.GetCurrentUserEmail() ?? "System",
                Timestamp = DateTime.Now,
                EmployeeId = employeeId
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }


        private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string? sortOrder)
        {
            // Default sort
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "LastName";
                sortOrder = "asc";
            }

            var isDescending = sortOrder?.ToLower() == "desc";

            query = sortBy.ToLower() switch
            {
                "firstname" => isDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
                "lastname" => isDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "department" => isDescending ? query.OrderByDescending(e => e.Department!.Name) : query.OrderBy(e => e.Department!.Name),
                "jobtitle" => isDescending ? query.OrderByDescending(e => e.JobTitle!.TitleName) : query.OrderBy(e => e.JobTitle!.TitleName),
                "salary" => isDescending ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary),
                "hiredate" => isDescending ? query.OrderByDescending(e => e.HireDate) : query.OrderBy(e => e.HireDate),
                "status" => isDescending ? query.OrderByDescending(e => e.IsActive) : query.OrderBy(e => e.IsActive),
                _ => query.OrderBy(e => e.LastName)
            };

            return query;
        }



    }
}
