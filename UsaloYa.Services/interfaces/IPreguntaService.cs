using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.interfaces
{
    public interface IPreguntaService
    {
        Task<List<Pregunta>> GetAllPreguntas(int userId);
        Task<bool> SavePreguntas(List<Pregunta> Preguntas);
    }
}
