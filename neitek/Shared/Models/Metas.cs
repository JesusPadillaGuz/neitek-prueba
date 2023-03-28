using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace neitek.Shared.Models
{
    public class Metas
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Tareas> Tareas { get; set; }
    }
}
