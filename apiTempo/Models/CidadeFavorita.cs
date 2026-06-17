namespace apiTempo.Models
{
    public class CidadeFavorita
    {
        public int Id { get; set; }
        public string NomeCidadeFavorita { get; set; }       
        public int? UsuarioId { get; set; }
    }
}