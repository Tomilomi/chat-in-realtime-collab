namespace Domain.Entity
{
    internal class Message
    {
        private Guid Id { get; set; }
        private User Sender { get; set; }
        private string Content { get; set; }
        private DateTime Timestamp { get; set; }

        //private int receiverId; maybe? al pedo
    }
}