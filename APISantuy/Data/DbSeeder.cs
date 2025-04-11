public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                FullName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var employeeUser = new User
            {
                FullName = "Employee User",
                Email = "employee@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = "Employee",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(adminUser, employeeUser);
            context.SaveChanges();
        }
    }
}
