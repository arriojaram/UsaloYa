using System.ComponentModel.DataAnnotations;

namespace UsaloYa.Dto
{
    public class RegisterUserQuestionnaireAndCompanyDto
    {
        [Required]
        public RequestRegisterNewUserDto RequestRegisterNewUserDto { get; set; }

        [Required]
        public CompanyDto CompanyDto { get; set; }
        [Required]
        public List<RequestSaveQuestionnaireDto> RequestSaveQuestionnaireDto { get; set; }

    }
}
