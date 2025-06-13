using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Library.Models
{
    public partial class Pregunta
    {
        public int PreguntaId { get; set; }
        public string Question { get; set; }
        public bool Answer { get; set; }
        public int? IdUser { get; set; }


        public User User { get; set; }  // Clase relacionada

    }
}
