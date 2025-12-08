using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    internal class User
    {
        private Guid Id { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private Picture Picture { get; set; }
    }
}
