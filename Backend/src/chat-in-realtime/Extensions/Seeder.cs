using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;

namespace chat_in_realtime.Extensions
{
    public static class Seeder

    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<AppDbContext>();
            var configuration = services.GetRequiredService<IConfiguration>();

            try
            {
                PrepareDatabase(db);

                var admin = SeedAdmin(db, configuration);

                SeedTest(db, admin);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar el Seeder: {ex.Message}");
            }
        }

        private static void PrepareDatabase(AppDbContext db)
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        private static User SeedAdmin(AppDbContext db, IConfiguration configuration)
        {
            var picture1 = new Picture("/avatars/avatar1.png");

            var adminConfig = configuration.GetSection("AdminConfig");
            string adminUsername = adminConfig["Username"] ?? "admin";
            string adminPassword = adminConfig["Password"] ?? "admin123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminPassword);

            var admin = User.Create(adminUsername, hashedPassword, picture1).Value;
            admin.ChangeRole(UserRole.Admin);

            db.Users.Add(admin);
            return admin;
        }

        public static void SeedTest(AppDbContext db, User admin)

        {
            var picture2 = new Picture("/avatars/avatar2.png");
            var picture3 = new Picture("/avatars/avatar3.png");
            db.Pictures.Add(picture3);

            var user2 = User.Create("Laura", BCrypt.Net.BCrypt.HashPassword("password123"), picture2).Value;
            db.Users.Add(user2);

            var messages = new List<Message>();
            for (int i = 1; i <= 50; i++)
            {
                var sender = i % 2 == 0 ? admin : user2;
                var msg = new Message(sender, $"Mensaje de prueba número {i}");

                msg.ForceTimestamp(DateTime.UtcNow.AddMinutes(-50 + i));
                messages.Add(msg);
            }

            db.Messages.AddRange(messages);
        }
    }
}