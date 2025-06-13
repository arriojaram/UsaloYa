using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
namespace UsaloYa.Services
{
    public class PreguntaService: IPreguntaService
    {
        private readonly DBContext _dBContext;
        public PreguntaService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<Pregunta>> GetAllPreguntas(int userId)
        {
            return await _dBContext.Preguntas
                .Where(p => p.IdUser == userId)
                .ToListAsync();
        }

        public async Task<bool> SavePreguntas(List<Pregunta> Preguntas)
        {
            try {
                 await _dBContext.Preguntas.AddRangeAsync(Preguntas);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch { 
                return false;
            }

        }

    }
}
