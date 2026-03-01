namespace Domain.Entities

{
    public class Message
    {
        public Guid Id { get; private set; }
        public User Sender { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Message(Guid id, User sender, string content, DateTime timestamp)
        {
            Id = id;
            Sender = sender;
            Content = content;
            Timestamp = timestamp;
        }
        
        private Message()
        {
            Sender = null!;
            Content = null!;
        }
    }
}