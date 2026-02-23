using Domain.Entity; 
// esto es para las clases de entidad
using Microsoft.EntityFrameworkCore; 
// esto es para usar el orm

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    // Clase que hereda de DbContext que representa la conexion a la base de datos
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // Constructor que recibe la cfg de la bdd, en program.cs
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        // Tablas de la bbd
        
    }
}