namespace Domain.Entities

{
    public class Message
    {
        public Guid Id { get; private set; }
        public User Sender { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Message(User sender, string content)
        {
            Id = Guid.NewGuid();
            Sender = sender;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }
        
        private Message()
        {
            Sender = null!;
            Content = null!;
        }
    }
}