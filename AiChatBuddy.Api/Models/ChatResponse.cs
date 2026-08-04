namespace AiChatBuddy.Api.Models;

/// <summary>
/// Odpowiedź endpointu czatu zwracana klientowi.
/// </summary>
public class ChatResponse
{
    // Wygenerowana przez model odpowiedź (markdown w formacie narzuconym promptem systemowym).
    public string Message { get; set; } = string.Empty;

    // Status obsługi żądania.
    public string Status { get; set; } = string.Empty;
}