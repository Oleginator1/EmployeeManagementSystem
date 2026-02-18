namespace EmployeeManagementSystem.Constants
{
    public static class Roles
    {

        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Employee = "Employee";

        
        public static List<string> GetAllRoles()
        {
            return new List<string> { Admin, Manager, Employee };
        }
    }
}
