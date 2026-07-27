using AiChatBuddy.Api.Models;
using Microsoft.Extensions.AI;
using ChatResponse = AiChatBuddy.Api.Models.ChatResponse;

namespace AiChatBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatClient chatClient) : ControllerBase
{
    [HttpPost]
    public async Task<ChatResponse> SendQuery([FromBody] ChatRequest request)
    {
        List<ChatMessage> messages = new()
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User, request.Query)
        };

        var response = await chatClient.GetResponseAsync(messages, new ChatOptions());

        return new ChatResponse
        {
            Message = response.Text,
            Status = "Success"
        };
    }
}