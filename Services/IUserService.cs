namespace EmployeeManagementSystem.Services
{
    public interface IUserService
    {
        string? GetCurrentUserEmail();

        string? GetCurrentUserId();

        Task<bool> IsInRoleAsync(string roleName);

        Task<IList<string>> GetUserRolesAsync();
    }
}
