using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UsaloYa.API.Security;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Config;
using UsaloYa.Library.Models;
using UsaloYa.Services;
using UsaloYa.Services.interfaces;


namespace UsaloYa.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class QuestionnaireController : ControllerBase
    {   
            private readonly ILogger<QuestionnaireController> _logger;
            private readonly IQuestionnaireService _preguntaService;
            private readonly IConfiguration _configuration;

            public QuestionnaireController(IQuestionnaireService preguntaService, ILogger<QuestionnaireController> logger, IConfiguration configuration)
            {
                _logger = logger;
                _preguntaService = preguntaService;
                _configuration = configuration;

            }

        [HttpPost("SaveQuestionnaire")]
        public async Task<IActionResult> SaveQuestionnaire([FromBody] List<RequestSaveQuestionnaire> preguntas)
        {
            try
            {

                var result = await _preguntaService.SaveQuestionnaire(preguntas);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLicenses.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida " + ex.Message });
            }
        }

        [HttpGet("GetQuestionnaireToAsk")]
        public IActionResult GetQuestionnaire()
        {
            try
            {
                var preguntas = _configuration.GetSection("QuestionnaireRegister").Get<List<string>>();
                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetPreguntas");
                return StatusCode(500, new { message = "Ocurrió una excepción al obtener las preguntas." });
            }
        }

        [HttpGet("GetQuestionnaireByUser")]
        public async Task<IActionResult> GetQuestionnaireByUser( int idUser)
        {
            try
            {
                var preguntas = await _preguntaService.GetQuestionnaireByUser(idUser);
                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetPreguntas");
                return StatusCode(500, new { message = "Ocurrió una excepción al obtener las preguntas del usuario." });
            }
        }


    }
}
