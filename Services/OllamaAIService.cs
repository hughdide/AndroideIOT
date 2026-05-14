using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AndroideIOT.Configuration;
using AndroideIOT.Models;
using Microsoft.Extensions.Options;

namespace AndroideIOT.Services;

public class OllamaAIService : IAIService
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

    public OllamaAIService(HttpClient httpClient, IOptions<AiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromMinutes(3);
    }

    public async Task<AiResponse?> GetResponseAsync(IReadOnlyList<ChatMessage> messages)
    {
        var ollamaMessages = messages
            .Select(m => new { role = m.Role, content = m.Content })
            .Prepend(new { role = "system", content = SystemPrompt })
            .ToArray();

        var body = new
        {
            model = _settings.Model,
            messages = ollamaMessages,
            stream = false,
            options = new { num_predict = _settings.MaxTokens }
        };

        var json = JsonSerializer.Serialize(body);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/chat", httpContent);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<OllamaResponse>(raw);
        var text = apiResponse?.Message?.Content?.Trim() ?? string.Empty;

        // Strip <think>...</think> block that deepseek-r1 emits before the response
        var thinkEnd = text.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
        if (thinkEnd >= 0)
            text = text[(thinkEnd + "</think>".Length)..].TrimStart();

        // Strip ```json ... ``` fences if the model adds them
        if (text.StartsWith("```"))
        {
            var lines = text.Split('\n');
            text = string.Join('\n', lines.Skip(1).TakeWhile(l => l.Trim() != "```"));
        }

        return JsonSerializer.Deserialize<AiResponse>(text);
    }

    private record OllamaResponse(
        [property: JsonPropertyName("message")] OllamaMessage? Message
    );

    private record OllamaMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content
    );
}
