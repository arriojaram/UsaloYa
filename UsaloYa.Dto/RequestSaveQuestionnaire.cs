using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RequestSaveQuestionnaire
    {
        public string Question { get; set; }
        public bool Answer { get; set; }
        public int? IdUser { get; set; }
    }
}
