using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Registrar MCP Client como Singleton
builder.Services.AddSingleton<Task<McpClient>>(async _ =>
{
    var transport = new StdioClientTransport(new StdioClientTransportOptions
    {
        Command = "dotnet",
        Arguments = new[]
        {
            "run",
            "--project",
            "C:/Users/Mario/RiderProjects/MauiIoT/McpServer/McpServer.csproj"
        }
    });

    return await McpClient.CreateAsync(transport);
});

// Registrar ChatService
builder.Services.AddScoped<ChatService>(provider =>
{
    var mcpClientTask = provider.GetRequiredService<Task<McpClient>>();
    var mcpClient = mcpClientTask.Result; // Obtener el McpClient del Task
    return new ChatService(mcpClient);
});

var app = builder.Build();

app.MapControllers();

app.Run();