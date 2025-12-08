using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    internal class Picture
    {
        private Guid Id { get; set; }
        private byte[] Data { get; set; }
        // No se si esta bien el tipo de dato
        // Debe ser un blob en la base de datos
    }
}
