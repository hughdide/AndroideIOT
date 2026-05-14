using System.Text.Json.Serialization;

namespace AndroideIOT.Models;

public class AiResponse
{
    [JsonPropertyName("respuesta_android")]
    public string RespuestaAndroid { get; set; } = string.Empty;

    [JsonPropertyName("metricas")]
    public AndroidMetrics Metricas { get; set; } = new();
}
