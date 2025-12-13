namespace Domain.Entity
{
    public class Picture
    {
        private Guid Id { get; set; }
        private byte[] Data { get; set; }
        // No se si esta bien el tipo de dato
        // Debe ser un blob en la base de datos
    }
}