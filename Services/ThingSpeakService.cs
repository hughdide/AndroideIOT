using System.Text.Json;
using System.Text.Json.Serialization;
using AndroideIOT.Configuration;
using AndroideIOT.Models;
using Microsoft.Extensions.Options;

namespace AndroideIOT.Services;

public class ThingSpeakService : IThingSpeakService
{
    private readonly HttpClient _httpClient;
    private readonly ThingSpeakSettings _settings;

    public ThingSpeakService(HttpClient httpClient, IOptions<ThingSpeakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<bool> SendMetricsAsync(int cargaCognitiva, int nivelCoherencia, int intensidadEmocional, int latenciaInferencia, int consumoEnergetico)
    {
        var url = $"{_settings.BaseUrl}/update?api_key={_settings.WriteApiKey}" +
                  $"&field1={cargaCognitiva}" +
                  $"&field2={nivelCoherencia}" +
                  $"&field3={intensidadEmocional}" +
                  $"&field4={latenciaInferencia}" +
                  $"&field5={consumoEnergetico}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return false;

        var body = await response.Content.ReadAsStringAsync();
        return int.TryParse(body.Trim(), out int entryId) && entryId > 0;
    }

    public async Task<AndroidMetrics?> GetLatestMetricsAsync()
    {
        var url = $"{_settings.BaseUrl}/channels/{_settings.ChannelId}/feeds/last.json";
        if (!string.IsNullOrEmpty(_settings.ReadApiKey))
            url += $"?api_key={_settings.ReadApiKey}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var feed = JsonSerializer.Deserialize<ThingSpeakFeed>(json);
        if (feed is null) return null;

        return new AndroidMetrics
        {
            CargaCognitiva      = ParseField(feed.Field1),
            NivelCoherencia     = ParseField(feed.Field2),
            IntensidadEmocional = ParseField(feed.Field3),
            LatenciaInferencia  = ParseField(feed.Field4),
            ConsumoEnergetico   = ParseField(feed.Field5),
        };
    }

    private static int ParseField(string? value) =>
        int.TryParse(value, out int n) ? n : 0;

    private record ThingSpeakFeed(
        [property: JsonPropertyName("field1")] string? Field1,
        [property: JsonPropertyName("field2")] string? Field2,
        [property: JsonPropertyName("field3")] string? Field3,
        [property: JsonPropertyName("field4")] string? Field4,
        [property: JsonPropertyName("field5")] string? Field5
    );
}
