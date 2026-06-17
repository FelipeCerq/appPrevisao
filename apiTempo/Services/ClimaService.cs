using System.Text.Json;
using apiTempo.Models;

namespace apiTempo.Services
{
    public class ClimaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public ClimaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ClimaResponse> ObterTemperaturaAtual(string cidade)
        {
            var chaveApi = _configuration["OpenWeather:ApiKey"];
            //Example of API call (vindo do email oficial do openweathermap):
            //api.openweathermap.org/data/2.5/weather?q=London,uk&APPID=da3e18af4a58f3cef2634b55ef0039b6
            // Para temperatura em Celsius e velocidade do vento em metro/seg, usar units=metric            
            var URLopenweathermap = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&APPID={chaveApi}";
            var response = await _httpClient.GetAsync(URLopenweathermap);

            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                 throw new Exception($"Falha ao consultar temperatura.");
            }
            var json = await response.Content.ReadAsStringAsync();

            using var documento = JsonDocument.Parse(json);
            return new ClimaResponse
            {
                Cidade = documento.RootElement.GetProperty("name").GetString(),
                Temperatura = documento.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
                TemperaturaMin = documento.RootElement.GetProperty("main").GetProperty("temp_min").GetDouble(),
                TemperaturaMax = documento.RootElement.GetProperty("main").GetProperty("temp_max").GetDouble(),
                Umidade = documento.RootElement.GetProperty("main").GetProperty("humidity").GetInt32(),
                Descricao = documento.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
            };
        }
        public async Task<List<PrevisaoResponse>> ObterPrevisao5dias(string cidade)
        {
            //Você pode pesquisar previsões do tempo para 5 dias com dados a cada 3 horas por coordenadas geográficas.
            //api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={API key}
            //Diferente do weather (precisa da lat e log), o forecast retorna os dados a partir da cidade ficando mais simples; 
            var chaveApi = _configuration["OpenWeather:ApiKey"];
            var URLopenweathermap = $"https://api.openweathermap.org/data/2.5/forecast?q={cidade}&units=metric&APPID={chaveApi}";
            var response = await _httpClient.GetAsync(URLopenweathermap);
                        
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                 throw new Exception($"Falha ao consultar clima.");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var documento = JsonDocument.Parse(json);
            var previsoes = new List<PrevisaoResponse>();

            foreach (var item in documento.RootElement.GetProperty("list").EnumerateArray())
            {                
                previsoes.Add(new PrevisaoResponse
                {
                    TemperaturaMin5dias = item.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    TemperaturaMax5dias = item.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    DataPrevista = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).DateTime,
                    Descricao = item.GetProperty("weather")[0].GetProperty("description").GetString(),
                    IconeTempo = item.GetProperty("weather")[0].GetProperty("icon").GetString(),
                });
            }
            return previsoes;     
        }      
        
    }
}