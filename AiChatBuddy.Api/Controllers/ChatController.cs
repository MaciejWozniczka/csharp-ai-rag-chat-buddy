using System.ComponentModel;
using AiChatBuddy.Api.Models;
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
        _chatOptions.Tools = [AIFunctionFactory.Create(SearchItemAsync)];
    }
    [HttpPost]
    public async Task<ChatResponse> SendQuery([FromBody] ChatRequest request)
    {
        List<ChatMessage> messages = new()
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User, request.Query)
        };

        var response = await _chatClient.GetResponseAsync(messages, new ChatOptions());

        return new ChatResponse
        {
            Message = response.Text,
            Status = "Success"
        };
    }

    [Description("Searches for items based on the provided query.")]
    private async Task<IEnumerable<string>> SearchItemAsync([Description("The query to search for.")] string searchQuery, CancellationToken cancellationToken)
    {
        var searchResults = _vectorCollection
            .SearchAsync(searchQuery, top: 5, cancellationToken: cancellationToken);

        return await searchResults
            .Select(r => r.Record.Content)
            .ToListAsync(cancellationToken);
    }
}