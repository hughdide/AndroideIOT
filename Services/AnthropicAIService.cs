using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AndroideIOT.Configuration;
using AndroideIOT.Models;
using Microsoft.Extensions.Options;

namespace AndroideIOT.Services;

public class AnthropicAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly AiSettings _settings;

    private const string SystemPrompt =
        "Eres un androide conversacional. Para cada mensaje responde ÚNICAMENTE con un JSON válido " +
        "con este formato exacto, sin texto adicional ni bloques de código markdown:\n" +
        "{\n" +
        "  \"respuesta_android\": \"tu respuesta aquí\",\n" +
        "  \"metricas\": {\n" +
        "    \"carga_cognitiva\": 50,\n" +
        "    \"nivel_coherencia\": 75,\n" +
        "    \"intensidad_emocional\": 60,\n" +
        "    \"latencia_inferencia\": 40,\n" +
        "    \"consumo_energetico\": 55\n" +
        "  }\n" +
        "}\n" +
        "Los valores de métricas deben estar entre 0 y 100 y reflejar el estado simulado del androide.";

    public AnthropicAIService(HttpClient httpClient, IOptions<AiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri("https://api.anthropic.com");
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }

    public async Task<AiResponse?> GetResponseAsync(IReadOnlyList<ChatMessage> messages)
    {
        var apiMessages = messages
            .Select(m => new { role = m.Role, content = m.Content })
            .ToArray();

        var body = new
        {
            model = _settings.Model,
            max_tokens = _settings.MaxTokens,
            system = SystemPrompt,
            messages = apiMessages
        };

        var json = JsonSerializer.Serialize(body);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/messages", httpContent);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<AnthropicResponse>(raw);
        var text = apiResponse?.Content?.FirstOrDefault()?.Text?.Trim() ?? string.Empty;

        // Strip ```json ... ``` fences if the model adds them
        if (text.StartsWith("```"))
        {
            var lines = text.Split('\n');
            text = string.Join('\n', lines.Skip(1).TakeWhile(l => l.Trim() != "```"));
        }

        return JsonSerializer.Deserialize<AiResponse>(text);
    }

    private record AnthropicResponse(
        [property: JsonPropertyName("content")] List<AnthropicContentBlock>? Content
    );

    private record AnthropicContentBlock(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("text")] string Text
    );
}
