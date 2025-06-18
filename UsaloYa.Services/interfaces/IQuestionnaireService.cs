using UsaloYa.Dto;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.interfaces
{
    public interface IQuestionnaireService
    {
        Task<List<QuestionDto>> GetAllQuestionnaires();
        Task<List<QuestionDto>> GetQuestionnaireByUser(int userId);
        Task<bool> SaveQuestionnaire(List<RequestSaveQuestionnaireDto> Preguntas);
    }
}
