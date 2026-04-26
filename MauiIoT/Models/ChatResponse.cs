namespace MauiIoT.Models;

class ChatResponse
{
    public string? Action { get; set; }
    public string? Result { get; set; }
    public string Message { get; set; } = "";
    public bool Success { get; set; }
}