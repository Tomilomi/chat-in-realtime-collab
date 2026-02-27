namespace Domain.Entities

{
    public class Message
    {
        public Guid Id { get; private set; }
        public User Sender { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        //private int receiverId; maybe? al pedo
    }
}