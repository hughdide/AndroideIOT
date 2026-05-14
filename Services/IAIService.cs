using AndroideIOT.Models;

namespace AndroideIOT.Services;

public interface IAIService
{
    Task<AiResponse?> GetResponseAsync(IReadOnlyList<ChatMessage> messages);
}
