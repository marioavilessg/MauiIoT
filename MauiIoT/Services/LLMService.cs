using System.Net.Http.Json;
using System.Text.Json;

public class LLMService
{
    private readonly HttpClient _http = new();

    public async Task<string?> ProcesarComando(string comando)
    {
        try
        {
            var request = new
            {
                model = "local-model",
                messages = new[]
                {
                    new {
                        role = "system",
                        content = "Responde SOLO con uno de estos valores: encender_luz_roja, encender_luz_verde, encender_luz_amarilla."
                    },
                    new { role = "user", content = comando }
                }
            };

            var response = await _http.PostAsJsonAsync(
                "http://localhost:1234/v1/chat/completions",
                request
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            return doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<bool> IsModelAvailable()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:1234/v1/models");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}