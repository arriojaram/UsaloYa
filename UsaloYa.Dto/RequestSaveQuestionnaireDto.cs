using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RequestSaveQuestionnaireDto
    {
        public string QuestionName { get; set; }
        public bool Reply { get; set; }
        public int? IdUser { get; set; }
    }
}
