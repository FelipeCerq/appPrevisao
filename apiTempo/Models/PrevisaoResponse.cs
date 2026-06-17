namespace apiTempo.Models
{
    public class PrevisaoResponse
    {   
        public double TemperaturaMin5dias { get; set; }
        public double TemperaturaMax5dias { get; set; }        
        public DateTime DataPrevista { get; set; }
        public string Descricao { get; set; }
        public string IconeTempo { get; set; }
    }
}