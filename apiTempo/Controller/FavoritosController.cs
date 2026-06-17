using Microsoft.AspNetCore.Mvc;
using apiTempo.Services;
using Microsoft.AspNetCore.Authorization;

namespace apiTempo.Controllers
{    
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]        
    public class FavoritosController : ControllerBase
    {
        private readonly FavoritoService _favoritoService;

        public FavoritosController(FavoritoService favoritoService)
        {
            _favoritoService = favoritoService;
        }
        // Listagem de cidades favoritas 
        [HttpGet]
        public async Task<IActionResult> ListarFavoritas()
        {
            var favoritas = await _favoritoService.ListarFavoritas();
            return Ok(favoritas);
        }
        // Add. de cidades favoritas         
        [HttpPost]                
        public async Task<IActionResult> AdicionarCidadeFavorita([FromBody] string nomeCidade)
        {
            var cidadeFavorita = await _favoritoService.AdicionarCidadeFavorita(nomeCidade);
            return Ok(cidadeFavorita);
        }
        // Remover cidades favoritas 
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverCidadeFavorita(int id)
        {
            var sucesso = await _favoritoService.RemoverCidadeFavorita(id);
            if (sucesso)
            {
                return NoContent();
            }
            return NotFound();
        }

    }

}