using AndroideIOT.Models;

namespace AndroideIOT.Services;

public interface IThingSpeakService
{
    Task<bool> SendMetricsAsync(int cargaCognitiva, int nivelCoherencia, int intensidadEmocional, int latenciaInferencia, int consumoEnergetico);
    Task<AndroidMetrics?> GetLatestMetricsAsync();
}
