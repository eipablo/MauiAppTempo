using MauiAppTempo.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempo.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "456d84610483c74f1868f6b32d1b6a9c";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&appid={chave}&units=metric&lang=pt_br";

            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"]).ToLocalTime().DateTime;
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunset"]).ToLocalTime().DateTime;

                    t = new()
                    {
                        lat = (double)(rascunho["coord"]["lat"]),
                        lon = (double)(rascunho["coord"]["lon"]),
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString("HH:mm"),
                        sunset = sunset.ToString("HH:mm"),
                    };
                }
                else if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    // Cidade não encontrada
                    throw new Exception("Cidade não encontrada. Verifique o nome digitado.");
                }
                else
                {
                    // Outros erros da API
                    throw new Exception($"Erro ao buscar dados do clima. Código: {(int)resp.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                // Sem internet
                throw new Exception("Sem conexão com a internet.");
            }

            return t;
        }
    }
}
