using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EmployeeManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<bool> IsInRoleAsync(string roleName)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return new List<string>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

    }
}
