using Microsoft.AspNetCore.Mvc;
using apiTempo.Services;

namespace apiTempo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClimaController : ControllerBase
    {
        private readonly ClimaService _climaService;

        public ClimaController(ClimaService climaService)
        {
            _climaService = climaService;
        }

        [HttpGet("{cidade}")]
        public async Task<IActionResult> GetTemperaturaAtual(string cidade)
        {
            var clima = await _climaService.ObterTemperaturaAtual(cidade);
            return Ok(clima);
        }

        [HttpGet("previsao/{cidade}")]
        public async Task<IActionResult> GetPrevisao5dias(string cidade)
        {
            var previsao = await _climaService.ObterPrevisao5dias(cidade);
            return Ok(previsao);
        }
    }
}