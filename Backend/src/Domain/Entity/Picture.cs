using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Picture
    {
        public Guid Id { get; set; }
        public byte[] Data { get; set; }
        // No se si esta bien el tipo de dato
        // Debe ser un blob en la base de datos
    }
}
