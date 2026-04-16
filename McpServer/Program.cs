using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Para usar HttpClient correctamente
builder.Services.AddHttpClient();

var app = builder.Build();


// TOOL: LUZ ROJA
app.MapPost("/tools/encender_luz_roja", async (IHttpClientFactory factory) =>
{
    var http = factory.CreateClient();

    await http.PostAsync("http://localhost:1880/leds",
        new StringContent("encender_rojo", Encoding.UTF8, "text/plain"));

    Console.WriteLine("Luz roja encendida");

    return Results.Ok("rojo");
});


// TOOL: LUZ VERDE
app.MapPost("/tools/encender_luz_verde", async (IHttpClientFactory factory) =>
{
    var http = factory.CreateClient();

    await http.PostAsync("http://localhost:1880/leds",
        new StringContent("encender_verde", Encoding.UTF8, "text/plain"));

    Console.WriteLine("Luz verde encendida");

    return Results.Ok("verde");
});


// TOOL: LUZ AMARILLA
app.MapPost("/tools/encender_luz_amarilla", async (IHttpClientFactory factory) =>
{
    var http = factory.CreateClient();

    await http.PostAsync("http://localhost:1880/leds",
        new StringContent("encender_amarillo", Encoding.UTF8, "text/plain"));

    Console.WriteLine("Luz amarilla encendida");

    return Results.Ok("amarilla");
});

app.MapGet("/tools", () => new[]
{
    "encender_luz_roja",
    "encender_luz_verde",
    "encender_luz_amarilla"
});


// PUERTO
app.Run("http://localhost:5000");