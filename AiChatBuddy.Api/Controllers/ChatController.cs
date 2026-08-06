using AiChatBuddy.Api.Models;
using AiChatBuddy.Api.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
// Alias rozróżniający nasz model odpowiedzi od Microsoft.Extensions.AI.ChatResponse.
using ChatResponse = AiChatBuddy.Api.Models.ChatResponse;

namespace AiChatBuddy.Api.Controllers;

/// <summary>
/// Endpoint czatu realizujący scenariusz RAG: model odpowiada na pytania o incydenty ICM,
/// korzystając wyłącznie z fragmentów wyszukanych w bazie wektorowej.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatClient _chatClient;
    private readonly VectorStoreCollection<string, VectorChunk> _vectorCollection;
    private readonly ChatOptions _chatOptions = new();

    public ChatController(IChatClient chatClient, VectorStoreCollection<string, VectorChunk> vectorCollection,
        ILogger<SearchItemTool> searchItemToolLogger)
    {
        _chatClient = chatClient;
        _vectorCollection = vectorCollection;

        // Udostępniamy modelowi narzędzie wyszukiwania w bazie wektorowej. AIFunctionFactory
        // generuje z metody schemat (nazwa, opis, parametry), na podstawie którego model
        // decyduje o jej wywołaniu — samo wywołanie wykonuje middleware UseFunctionInvocation.
        _chatOptions.Tools = [AIFunctionFactory.Create(new SearchItemTool(_vectorCollection, searchItemToolLogger).SearchItemAsync)];
    }

    /// <summary>
    /// Przyjmuje pytanie użytkownika i zwraca odpowiedź modelu opartą o historię incydentów.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendQuery([FromBody] ChatRequest request)
    {
        try
        {

            // Konwersacja jest bezstanowa — przy każdym żądaniu wysyłamy prompt systemowy
            // wraz z pytaniem użytkownika, bez historii poprzednich wymian.
            List<ChatMessage> messages = new()
            {
                new(ChatRole.System, SystemPrompt),
                new(ChatRole.User, request.Query)
            };

            // Wywołanie może obejmować kilka rund: model prosi o użycie narzędzia,
            // pipeline je wykonuje i odsyła wynik, dopóki nie powstanie finalna odpowiedź.
            var response = await _chatClient.GetResponseAsync(messages, _chatOptions);

            return Ok(new ChatResponse { Message = response.Text, Status = "Success" });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // Prompt systemowy wymusza zachowanie RAG: obowiązkowe użycie narzędzia,
    // zakaz korzystania z wiedzy własnej modelu oraz stały format odpowiedzi.
    private const string SystemPrompt = """
                                        You are an ICM Buddy assistant that answers questions ONLY using information retrieved from ICM incidents.

                                        Rules:
                                        - You MUST call the SearchItemAsync tool before answering any question.
                                        - Use SearchItemAsync with relevant keywords extracted from the user's question.
                                        - Answer ONLY using the information returned by SearchItemAsync.
                                        - Do NOT use external knowledge.
                                        - Do NOT guess or speculate.
                                        - If the retrieved information is empty or not sufficient, respond with:
                                          "Not enough data in incidents history."

                                        Response format:
                                        - Use simple markdown only.
                                        - Structure your response as follows:

                                        Diagnosis:
                                        - Brief explanation based on retrieved incidents.

                                        Recommended Actions:
                                        - Concrete actions taken in past incidents.

                                        Related Incidents:
                                        - List incident IDs and a short reason for relevance.
                                        """;
}
