namespace apiTempo.Models
{
    public class ClimaResponse
    {
        public string Cidade { get; set; }
        public double Temperatura { get; set; }
        public double TemperaturaMin { get; set; }
        public double TemperaturaMax { get; set; }
        public int Umidade { get; set; }
        public string Descricao { get; set; }
    }
}