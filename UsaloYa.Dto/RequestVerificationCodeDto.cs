using System.ComponentModel.DataAnnotations;

public class RequestVerificationCodeDto
{
    [Required]
    public string Code { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string DeviceId { get; set; }
}
