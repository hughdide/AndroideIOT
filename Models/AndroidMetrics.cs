using System.Text.Json.Serialization;

namespace AndroideIOT.Models;

public class AndroidMetrics
{
    [JsonPropertyName("carga_cognitiva")]
    public int CargaCognitiva { get; set; }

    [JsonPropertyName("nivel_coherencia")]
    public int NivelCoherencia { get; set; }

    [JsonPropertyName("intensidad_emocional")]
    public int IntensidadEmocional { get; set; }

    [JsonPropertyName("latencia_inferencia")]
    public int LatenciaInferencia { get; set; }

    [JsonPropertyName("consumo_energetico")]
    public int ConsumoEnergetico { get; set; }
}
