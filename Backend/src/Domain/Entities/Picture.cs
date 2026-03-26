
namespace Domain.Entities
{
    public class Picture
    {
        public Guid Id { get; private set; }
        public string Url { get; private set; }
        
        // Constructores agregados
        
        public Picture(string url)
        {
            Id = Guid.NewGuid();
            Url = url;
        }
        
        private Picture() 
        {
            Url = null!;
        }
    }
}