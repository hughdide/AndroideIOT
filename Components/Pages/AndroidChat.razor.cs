using AndroideIOT.Models;
using AndroideIOT.Services;
using AndroideIOT.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace AndroideIOT.Components.Pages;

public partial class AndroidChat
{
    [Inject] private IAIService AIService { get; set; } = default!;
    [Inject] private IThingSpeakService ThingSpeakService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    protected AndroidChatViewModel VM { get; } = new();

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(VM.TextoUsuario) && !VM.Cargando)
            await EnviarMensaje();
    }

    private async Task EnviarMensaje()
    {
        var texto = VM.TextoUsuario.Trim();
        if (string.IsNullOrEmpty(texto)) return;

        VM.TextoUsuario = string.Empty;
        VM.ErrorMensaje = null;
        VM.ThingSpeakOk = null;

        VM.Mensajes.Add(new ChatMessage { Role = "user", Content = texto });
        VM.Cargando = true;
        await ScrollAbajo();

        try
        {
            var respuesta = await AIService.GetResponseAsync(VM.Mensajes.AsReadOnly());

            if (respuesta is not null)
            {
                VM.Mensajes.Add(new ChatMessage { Role = "assistant", Content = respuesta.RespuestaAndroid });
                VM.Metricas = respuesta.Metricas;
                await GuardarEnThingSpeakAsync(VM.Metricas);
            }
            else
            {
                VM.ErrorMensaje = "El androide no devolvió respuesta. Intenta de nuevo.";
            }
        }
        catch (Exception ex)
        {
            VM.ErrorMensaje = $"Error: {ex.Message}";
        }
        finally
        {
            VM.Cargando = false;
            await ScrollAbajo();
        }
    }

    private async Task GuardarEnThingSpeakAsync(AndroidMetrics m)
    {
        VM.EnviandoThingSpeak = true;
        await InvokeAsync(StateHasChanged);
        try
        {
            VM.ThingSpeakOk = await ThingSpeakService.SendMetricsAsync(
                m.CargaCognitiva, m.NivelCoherencia,
                m.IntensidadEmocional, m.LatenciaInferencia,
                m.ConsumoEnergetico);

            if (VM.ThingSpeakOk == true)
            {
                // Espera a que ThingSpeak procese la escritura antes de leer
                await Task.Delay(1500);
                var confirmadas = await ThingSpeakService.GetLatestMetricsAsync();
                if (confirmadas is not null)
                    VM.Metricas = confirmadas;
            }
        }
        catch
        {
            VM.ThingSpeakOk = false;
        }
        finally
        {
            VM.EnviandoThingSpeak = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ScrollAbajo()
    {
        await InvokeAsync(StateHasChanged);
        await JS.InvokeVoidAsync("scrollToBottom", "chatMessages");
    }
}
