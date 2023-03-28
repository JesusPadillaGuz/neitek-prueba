using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace neitek.Shared.Models
{
    public class Tareas
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "El Nombre esta vacío")]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Status { get; set; }
        public bool Importante { get; set; }
        public int MetaID { get; set; }
        public Metas Meta { get; set; }
    }
}
