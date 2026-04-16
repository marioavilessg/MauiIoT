public class McpClientService
{
    private readonly HttpClient _http = new();

    public async Task Ejecutar(string tool)
    {
        await _http.PostAsync($"http://localhost:5000/tools/{tool}", null);
    }
}