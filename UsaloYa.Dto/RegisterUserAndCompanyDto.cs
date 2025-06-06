using System.ComponentModel.DataAnnotations;

namespace UsaloYa.Dto
{
    public class RegisterUserAndCompanyDto
    {
        [Required]
        public RequestRegisterNewUserDto RequestRegisterNewUserDto { get; set; }

        [Required]
        public CompanyDto CompanyDto { get; set; }

        [Required]
        public GroupDto GroupDto { get; set; }
    }
}
