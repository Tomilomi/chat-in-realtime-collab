using Domain.Entities;

// esto es para las clases de entidad
using Microsoft.EntityFrameworkCore;

// esto es para usar el orm

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    // Clase que hereda de DbContext que representa la conexion a la base de datos
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Constructor que recibe la cfg de la bdd, en program.cs
        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        // Tablas de la bbd

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos a EF Core cómo es la relación explícitamente
            modelBuilder.Entity<User>()
                .HasOne(u => u.Picture)      // Un usuario tiene una foto
                .WithOne(p => p.User)        // Una foto pertenece a un usuario
                .HasForeignKey<Picture>(p => p.UserId); // La tabla Picture guarda la Foreign Key
        }
    }
}