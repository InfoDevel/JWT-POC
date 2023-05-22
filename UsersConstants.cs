namespace JWT_POC
{
    public class UsersConstants
    {
        public static readonly List<User> Users = new List<User>()
        { 
            new User() {UserName = "user1", Email = "user1@email.com", Password = "pass", Role = "Admin"},
            new User() {UserName = "user2", Email = "user2@email.com", Password = "pass", Role = "Employee"},
        };

        public static readonly List<string> Roles = new List<string>()
        {
            "Admin", "Employee", "Supervisor", "Accounting", "copy-guy"
        };

    }
}
