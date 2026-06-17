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

        public FavoritosController(FavoritoService favoritoService)
        {
            _favoritoService = favoritoService;
        }


        // Listagem de cidades favoritas 
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
        public async Task<IActionResult> AdicionarCidadeFavorita([FromBody] string nomeCidade)
        {            
            var UsuCod = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(UsuCod))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var cidadeFavorita = await _favoritoService.AdicionarCidadeFavorita(nomeCidade, Convert.ToInt32(UsuCod));
            return Ok(cidadeFavorita);
        }
        // Remover cidades favoritas 
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