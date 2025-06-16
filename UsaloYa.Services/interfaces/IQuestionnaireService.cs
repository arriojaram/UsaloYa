using UsaloYa.Dto;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.interfaces
{
    public interface IQuestionnaireService
    {
        Task<List<Pregunta>> GetAllQuestionnaires();
        Task<List<Pregunta>> GetQuestionnaireByUser(int userId);
        Task<bool> SaveQuestionnaire(List<RequestSaveQuestionnaire> Preguntas);
    }
}
