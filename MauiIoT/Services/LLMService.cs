using System.Net.Http.Json;
using System.Text.Json;

public class LLMService
{
    private readonly HttpClient _http = new();

    public async Task<string> ProcesarComando(string comando)
    {
        var request = new
        {
            model = "local-model",
            messages = new[]
            {
                new { role = "system", content = "Eres un asistente que usa funciones para controlar luces." },
                new { role = "user", content = comando }
            },
            tools = new[]
            {
                new {
                    type = "function",
                    function = new {
                        name = "encender_luz_roja",
                        description = "Enciende la luz roja",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "encender_luz_verde",
                        description = "Enciende la luz verde",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "encender_luz_amarilla",
                        description = "Enciende la luz amarilla",
                        parameters = new { type = "object", properties = new { } }
                    }
                }
            },
            tool_choice = "auto"
        };

        var response = await _http.PostAsJsonAsync(
            "http://localhost:1234/v1/chat/completions",
            request
        );

        var json = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(json);

            var message = doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message");

            // 🔥 CLAVE: leer tool_calls
            if (message.TryGetProperty("tool_calls", out var toolCalls))
            {
                var toolName = toolCalls[0]
                    .GetProperty("function")
                    .GetProperty("name")
                    .GetString();

                return toolName ?? "";
            }

            // fallback por si el modelo no usa tools
            var content = message.GetProperty("content").GetString();
            return content ?? "";
        }
        catch
        {
            return "";
        }
    }
}