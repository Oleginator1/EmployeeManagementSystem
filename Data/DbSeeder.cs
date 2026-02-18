using EmployeeManagementSystem.Constants;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementSystem.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = Roles.GetAllRoles();

            foreach (var roleName in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                    Console.WriteLine($"Role '{roleName}' created successfully");
                }

            }
        }

        private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            string adminEmail = "admin@company.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, Roles.Admin);
                    Console.WriteLine($"Admin user created: {adminEmail}");
                    Console.WriteLine($"Password: {adminPassword}");
                    Console.WriteLine("Please change this password after first login!");
                }
                else
                {
                   
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
               
                if (!adminUser.LockoutEnabled)
                {
                    adminUser.LockoutEnabled = true;
                    await userManager.UpdateAsync(adminUser);
                    Console.WriteLine("Updated admin user: LockoutEnabled set to true");
                }
            }
        }
    }
}
