using Microsoft.EntityFrameworkCore;
using Rebex.Mail;
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

        public async Task<List<QuestionDto>> GetAllQuestionnaires()
        {
            
                return await _dBContext.Questions.
                    OrderBy(u => u.IdUser).Select(r => new QuestionDto
                    {

                        QuestionId = r.QuestionId,
                        QuestionName = r.QuestionName,
                        Reply = r.Reply,
                        IdUser = r.IdUser

                    })
                    .ToListAsync();
               

        }

        public async Task<List<QuestionDto>> GetQuestionnaireByUser(int userId)
        {
            return await _dBContext.Questions
                .Where(p => p.IdUser == userId)
                .Select(r => new QuestionDto
                {

                    QuestionId = r.QuestionId,
                    QuestionName = r.QuestionName,
                    Reply = r.Reply,
                    IdUser = r.IdUser

                })
                    .ToListAsync();
        }

        public async Task<bool> SaveQuestionnaire(List<RequestSaveQuestionnaire> preguntas)
        {
            try
            {
                var pregunta = preguntas.Select(p => new Question
                {
                    QuestionName = p.QuestionName,
                    Reply = p.Reply,
                    IdUser = p.IdUser
                }).ToList();
                await _dBContext.Questions.AddRangeAsync(pregunta);
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
