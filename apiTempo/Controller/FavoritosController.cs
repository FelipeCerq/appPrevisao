using Microsoft.AspNetCore.Mvc;
using apiTempo.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace apiTempo.Controllers
{    
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]        
    public class FavoritosController : ControllerBase
    {
        private readonly FavoritoService _favoritoService;
        private readonly ClimaService _climaService;

        public FavoritosController(FavoritoService favoritoService, ClimaService climaService)
        {
            _favoritoService = favoritoService;
            _climaService = climaService;
        }

        public class CidadeFavoritaAdd
        {
            public string NomeCidade { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> ListarFavoritas()
        {

            var UsuCod = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(UsuCod))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var favoritas = await _favoritoService.ListarFavoritas(Convert.ToInt32(UsuCod));
            return Ok(favoritas);
        }
        // Add. de cidades favoritas         
        [HttpPost]                
        public async Task<IActionResult> AdicionarCidadeFavorita([FromBody] CidadeFavoritaAdd request)
        {            
            var UsuCod = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(UsuCod))
            {
                return Unauthorized("Usuário não autenticado");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.NomeCidade))
            {
                return BadRequest("O nome da cidade não pode estar vazia.");
            }

            try
            {
                // Verifica se a cidade existe antes de incluir no banco o local.
                await _climaService.ObterTemperaturaAtual(request.NomeCidade);        
                var cidadeFavorita = await _favoritoService.AdicionarCidadeFavorita(request.NomeCidade, Convert.ToInt32(UsuCod));
                return Ok(cidadeFavorita);
            } 
            catch (InvalidOperationException)
            {
                return BadRequest("Essa cidade já foi adicionada no seu favorito");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverCidadeFavorita(int id)
        {             
            var UsuCod = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(UsuCod))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var sucesso = await _favoritoService.RemoverCidadeFavorita(id, Convert.ToInt32(UsuCod));
            if (sucesso)
            {
                return NoContent();
            }
            return NotFound();
        }

    }

}