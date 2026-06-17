
using Microsoft.EntityFrameworkCore;
using apiTempo.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace apiTempo.Services
{
    public class FavoritoService
    {
    private readonly AppDbContext _context;    
    private readonly IConfiguration _configuration;

    public FavoritoService(AppDbContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;        
    }

    public async Task<List<CidadeFavorita>> ListarFavoritas(int usuarioId)
    {
          await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

          var ListarFavoritos = await connection.QueryAsync<CidadeFavorita>(
              "SELECT Id, Nome, UsuarioId FROM CidadesFavoritas WHERE UsuarioId = @UsuarioId",
              new { UsuarioId = usuarioId }              
          );          
          return ListarFavoritos.ToList();
    }

    public async Task<bool> RemoverCidadeFavorita(int id, int usuarioId)
    {        
        var cidadeFavorita = await _context.CidadesFavoritas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == usuarioId);
        if (cidadeFavorita != null)
        {
            _context.CidadesFavoritas.Remove(cidadeFavorita);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;

    }

    public async Task<CidadeFavorita> AdicionarCidadeFavorita(string nomeCidade, int usuarioId)
    {           
        if(string.IsNullOrWhiteSpace(nomeCidade))
        {
            throw new ArgumentException("O nome da cidade não pode estar vazio.");
        }

        if(await _context.CidadesFavoritas.AnyAsync(x => x.Nome.ToLower() == nomeCidade.ToLower() && x.UsuarioId == usuarioId))
        {
            throw new InvalidOperationException("A cidade escolhida já existe na lista de favoritos.");
        }
        
        var cidadeFavorita = new CidadeFavorita { Nome = nomeCidade, UsuarioId = usuarioId };
        _context.CidadesFavoritas.Add(cidadeFavorita);

        await _context.SaveChangesAsync();
        return cidadeFavorita;        
    }
    }
}