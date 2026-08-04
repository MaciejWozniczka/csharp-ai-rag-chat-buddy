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
}