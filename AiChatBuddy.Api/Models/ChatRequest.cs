namespace AiChatBuddy.Api.Models;

/// <summary>
/// Treść żądania wysyłanego do endpointu czatu.
/// </summary>
public class ChatRequest
{
    // Pytanie użytkownika przekazywane modelowi jako wiadomość w roli "user".
    public string Query { get; set; } = string.Empty;
}