using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Message
    {
        public Guid Id { get; set; }
        public User Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        //private int receiverId; maybe? al pedo
    }
}
