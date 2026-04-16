public class IoTService
{
    private readonly HttpClient _http = new();

    public async Task EncenderLuz(string color)
    {
        var content = new StringContent(
            $"encender_{color}",
            System.Text.Encoding.UTF8,
            "text/plain"
        );

        await _http.PostAsync("http://localhost:1880/leds", content);
    }
}