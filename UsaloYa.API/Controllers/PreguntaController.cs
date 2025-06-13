using Microsoft.AspNetCore.Mvc;
using UsaloYa.Library.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using UsaloYa.Services;


namespace UsaloYa.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PreguntaController : ControllerBase
    {   
        private readonly ILogger<PreguntaController> _logger;
        private readonly IPreguntaService _preguntaService;
        private readonly IConfiguration _configuration;

        public PreguntaController(IPreguntaService preguntaService, ILogger<PreguntaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _preguntaService = preguntaService;
            _configuration = configuration;

        }

        [HttpPost("SavePreguntas")]
        public async Task<IActionResult> SavePreguntas([FromBody] List<Pregunta> preguntas)
        {
            try
            {

                var result = await _preguntaService.SavePreguntas(preguntas);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLicenses.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetPreguntas")]
        public IActionResult GetPreguntas()
        {
            try
            {
                var preguntas = _configuration.GetSection("Preguntas").Get<List<string>>();
                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetPreguntas");
                return StatusCode(500, new { message = "Ocurrió una excepción al obtener las preguntas." });
            }
        }
    }
}
