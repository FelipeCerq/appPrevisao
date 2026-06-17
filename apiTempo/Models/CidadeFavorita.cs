namespace apiTempo.Models
{
    public class CidadeFavorita
    {
        public int Id { get; set; }
        public string Nome { get; set; }       
        public int? UsuarioId { get; set; }
    }
}