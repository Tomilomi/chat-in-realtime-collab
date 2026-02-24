
namespace Domain.Entities
{
    public class Picture
    {
        public Guid Id { get; private set; }
        public byte[] Data { get; private set; }

        // No se si esta bien el tipo de dato
        // Debe ser un blob en la base de datos

        public Guid UserId { get; private set; }

        //propiedad de navegacion
        public User User { get; private set; }
    }
}