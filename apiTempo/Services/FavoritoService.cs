using Microsoft.EntityFrameworkCore;
using apiTempo.Models;

namespace apiTempo.Services
{
    public class FavoritoService
    {
    private readonly AppDbContext _context;    

    public FavoritoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CidadeFavorita>> ListarFavoritas()
    {
          return await _context.CidadesFavoritas.ToListAsync();
    }

    public async Task<bool> RemoverCidadeFavorita(int id)
    {        
        var cidadeFavorita = await _context.CidadesFavoritas.FirstOrDefaultAsync(x => x.Id == id);
        if (cidadeFavorita != null)
        {
            _context.CidadesFavoritas.Remove(cidadeFavorita);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;

    }

    public async Task<CidadeFavorita> AdicionarCidadeFavorita(string nomeCidade)
    {           
        if(string.IsNullOrWhiteSpace(nomeCidade))
        {
            throw new ArgumentException("O nome da cidade não pode estar vazio.");
        }

        if(await _context.CidadesFavoritas.AnyAsync(x => x.NomeCidadeFavorita.ToLower() == nomeCidade.ToLower()))
        {
            throw new InvalidOperationException("A cidade escolhida já existe na lista de favoritos.");
        }
        
        var cidadeFavorita = new CidadeFavorita { NomeCidadeFavorita = nomeCidade };
        _context.CidadesFavoritas.Add(cidadeFavorita);

        await _context.SaveChangesAsync();
        return cidadeFavorita;        
    }
    }
}