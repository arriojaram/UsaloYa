using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;


namespace UsaloYa.Services
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly DBContext _dBContext;
        public QuestionnaireService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<Pregunta>> GetAllQuestionnaires()
        {
            
                return await _dBContext.Preguntas.
                    OrderBy(u => u.IdUser).ToListAsync();
               

        }

        public async Task<List<Pregunta>> GetQuestionnaireByUser(int userId)
        {
            return await _dBContext.Preguntas
                .Where(p => p.IdUser == userId)
                .ToListAsync();
        }

        public async Task<bool> SaveQuestionnaire(List<RequestSaveQuestionnaire> preguntas)
        {
            try
            {
                var pregunta = preguntas.Select(p => new Pregunta
                {
                    Question = p.Question,
                    Answer = p.Answer,
                    IdUser = p.IdUser
                }).ToList();
                await _dBContext.Preguntas.AddRangeAsync(pregunta);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception("Error al registrar las reguntas"); ;
            }

        }
    }
}
