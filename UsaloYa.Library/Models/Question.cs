using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Library.Models
{
    public partial class Question

    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public bool Reply { get; set; }
        public int? IdUser { get; set; }


        public User User { get; set; }  // Clase relacionada

    }
}
