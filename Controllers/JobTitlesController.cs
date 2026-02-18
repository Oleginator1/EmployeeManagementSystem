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
    public class JobTitlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IPdfService _pdfService;

        public JobTitlesController(ApplicationDbContext context, IUserService userService, IPdfService pdfService)
        {
            _context = context;
            _userService = userService;
            _pdfService = pdfService;
        }


        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, string? sortOrder)
        {
            
            var query = _context.JobTitles
                .Include(j => j.Employees)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(j =>
                    j.TitleName.ToLower().Contains(search) ||
                    (j.Description != null && j.Description.ToLower().Contains(search))
                );
            }

           
            sortBy = sortBy?.ToLower() ?? "title";
            sortOrder = sortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "title" => sortOrder == "desc" ? query.OrderByDescending(j => j.TitleName) : query.OrderBy(j => j.TitleName),
                "employeecount" => sortOrder == "desc" ? query.OrderByDescending(j => j.Employees.Count) : query.OrderBy(j => j.Employees.Count),
                "minsalary" => sortOrder == "desc" ? query.OrderByDescending(j => j.MinSalary) : query.OrderBy(j => j.MinSalary),
                "maxsalary" => sortOrder == "desc" ? query.OrderByDescending(j => j.MaxSalary) : query.OrderBy(j => j.MaxSalary),
                "created" => sortOrder == "desc" ? query.OrderByDescending(j => j.CreatedDate) : query.OrderBy(j => j.CreatedDate),
                _ => query.OrderBy(j => j.TitleName)
            };

            var jobTitles = await query.ToListAsync();

           
            var viewModel = jobTitles.Select(j => new JobTitleViewModel
            {
                JobTitleId = j.JobTitleId,
                TitleName = j.TitleName,
                Description = j.Description,
                EmployeeCount = j.Employees.Count,
                ActiveEmployeeCount = j.Employees.Count(e => e.IsActive),
                CreatedDate = j.CreatedDate
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

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                    .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(m => m.JobTitleId == id);

            if (jobTitle == null)
            {
                return NotFound();
            }

            return View(jobTitle);
        }

       
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobTitleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                
                if (viewModel.MinSalary.HasValue && viewModel.MaxSalary.HasValue)
                {
                    if (viewModel.MinSalary > viewModel.MaxSalary)
                    {
                        ModelState.AddModelError("MaxSalary", "Maximum salary must be greater than minimum salary.");
                        return View(viewModel);
                    }
                }

                
                var exists = await _context.JobTitles
                    .AnyAsync(j => j.TitleName.ToLower() == viewModel.TitleName.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("TitleName", "A job title with this name already exists.");
                    return View(viewModel);
                }

                
                var jobTitle = new JobTitle
                {
                    TitleName = viewModel.TitleName,
                    Description = viewModel.Description,
                    MinSalary = viewModel.MinSalary,
                    MaxSalary = viewModel.MaxSalary,
                    CreatedDate = DateTime.Now
                };

                _context.Add(jobTitle);
                await _context.SaveChangesAsync();

                
                await LogActionAsync($"Created job title: {jobTitle.TitleName}");

                TempData["SuccessMessage"] = $"Job title '{jobTitle.TitleName}' created successfully!";
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

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.JobTitleId == id);

            if (jobTitle == null)
            {
                return NotFound();
            }

            var viewModel = new EditJobTitleViewModel
            {
                JobTitleId = jobTitle.JobTitleId,
                TitleName = jobTitle.TitleName,
                Description = jobTitle.Description,
                MinSalary = jobTitle.MinSalary,
                MaxSalary = jobTitle.MaxSalary,
                EmployeeCount = jobTitle.Employees.Count
            };

            return View(viewModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditJobTitleViewModel viewModel)
        {
            if (id != viewModel.JobTitleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validate salary range
                if (viewModel.MinSalary.HasValue && viewModel.MaxSalary.HasValue)
                {
                    if (viewModel.MinSalary > viewModel.MaxSalary)
                    {
                        ModelState.AddModelError("MaxSalary", "Maximum salary must be greater than minimum salary.");
                        return View(viewModel);
                    }
                }

                try
                {
                    var jobTitle = await _context.JobTitles.FindAsync(id);

                    if (jobTitle == null)
                    {
                        return NotFound();
                    }

                   
                    if (jobTitle.TitleName != viewModel.TitleName)
                    {
                        var exists = await _context.JobTitles
                            .AnyAsync(j => j.TitleName.ToLower() == viewModel.TitleName.ToLower() && j.JobTitleId != id);

                        if (exists)
                        {
                            ModelState.AddModelError("TitleName", "A job title with this name already exists.");
                            return View(viewModel);
                        }
                    }

                    // Update properties
                    jobTitle.TitleName = viewModel.TitleName;
                    jobTitle.Description = viewModel.Description;
                    jobTitle.MinSalary = viewModel.MinSalary;
                    jobTitle.MaxSalary = viewModel.MaxSalary;
                    jobTitle.ModifiedDate = DateTime.Now;

                    _context.Update(jobTitle);
                    await _context.SaveChangesAsync();

                    
                    await LogActionAsync($"Updated job title: {jobTitle.TitleName}");

                    TempData["SuccessMessage"] = $"Job title '{jobTitle.TitleName}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await JobTitleExistsAsync(viewModel.JobTitleId))
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

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(m => m.JobTitleId == id);

            if (jobTitle == null)
            {
                return NotFound();
            }

           
            if (jobTitle.Employees.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete job title '{jobTitle.TitleName}' because it has {jobTitle.Employees.Count} employee(s) assigned to it. Please reassign or remove employees first.";
                return RedirectToAction(nameof(Index));
            }

            return View(jobTitle);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.JobTitleId == id);

            if (jobTitle == null)
            {
                return NotFound();
            }

            
            if (jobTitle.Employees.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete job title '{jobTitle.TitleName}' because it has employees assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            var titleName = jobTitle.TitleName;

            _context.JobTitles.Remove(jobTitle);
            await _context.SaveChangesAsync();

           
            await LogActionAsync($"Deleted job title: {titleName}");

            TempData["SuccessMessage"] = $"Job title '{titleName}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ExportPdf(string? searchTerm, string? sortBy, string? sortOrder)
        {
            var query = _context.JobTitles
                .Include(j => j.Employees)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(j =>
                    j.TitleName.ToLower().Contains(search) ||
                    (j.Description != null && j.Description.ToLower().Contains(search))
                );
            }

           
            sortBy = sortBy?.ToLower() ?? "title";
            sortOrder = sortOrder?.ToLower() ?? "asc";

            query = sortBy switch
            {
                "title" => sortOrder == "desc" ? query.OrderByDescending(j => j.TitleName) : query.OrderBy(j => j.TitleName),
                "employeecount" => sortOrder == "desc" ? query.OrderByDescending(j => j.Employees.Count) : query.OrderBy(j => j.Employees.Count),
                _ => query.OrderBy(j => j.TitleName)
            };

            var jobTitles = await query.ToListAsync();

            var pdfBytes = _pdfService.GenerateJobTitleReport(jobTitles);

          
            await LogActionAsync($"Exported {jobTitles.Count} job titles to PDF");

            var fileName = $"JobTitles_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }



        //Helper Methods

        private async Task<bool> JobTitleExistsAsync(int id)
        {
            return await _context.JobTitles.AnyAsync(e => e.JobTitleId == id);
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