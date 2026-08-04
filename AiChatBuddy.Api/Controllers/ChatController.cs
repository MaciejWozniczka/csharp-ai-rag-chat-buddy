using AiChatBuddy.Api.Models;
using AiChatBuddy.Api.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ChatResponse = AiChatBuddy.Api.Models.ChatResponse;

namespace AiChatBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatClient _chatClient;
    private readonly VectorStoreCollection<string, VectorChunk> _vectorCollection;
    private readonly ChatOptions _chatOptions = new();
    public ChatController(IChatClient chatClient, VectorStoreCollection<string, VectorChunk> vectorCollection)
    {
        _chatClient = chatClient;
        _vectorCollection = vectorCollection;
        _chatOptions.Tools = [AIFunctionFactory.Create(new SearchItemTool(_vectorCollection).SearchItemAsync)];
    }
    [HttpPost]
    public async Task<ChatResponse> SendQuery([FromBody] ChatRequest request)
    {
        List<ChatMessage> messages = new()
        {
            new ChatMessage(ChatRole.System, SystemPrompt),
            new ChatMessage(ChatRole.User, request.Query)
        };

        var response = await _chatClient.GetResponseAsync(messages, _chatOptions);

        return new ChatResponse
        {
            Message = response.Text,
            Status = "Success"
        };
    }

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