using AndroideIOT.Services;
using AndroideIOT.ViewModels;
using Microsoft.AspNetCore.Components;

namespace AndroideIOT.Components.Pages;

public partial class ThingSpeakManual
{
    [Inject] private IThingSpeakService ThingSpeakService { get; set; } = default!;

    protected ThingSpeakManualViewModel VM { get; } = new();

    private async Task EnviarAsync()
    {
        VM.Enviando = true;
        VM.MensajeEstado = string.Empty;

        bool ok = await ThingSpeakService.SendMetricsAsync(
            VM.CargaCognitiva,
            VM.NivelCoherencia,
            VM.IntensidadEmocional,
            VM.LatenciaInferencia,
            VM.ConsumoEnergetico);

        VM.Enviando = false;

        if (ok)
        {
            VM.AlertClass = "alert-success";
            VM.MensajeEstado = "Datos enviados correctamente a ThingSpeak.";
        }
        else
        {
            VM.AlertClass = "alert-danger";
            VM.MensajeEstado = "Error al enviar. Comprueba la API Key y la conexión.";
        }
    }
}
