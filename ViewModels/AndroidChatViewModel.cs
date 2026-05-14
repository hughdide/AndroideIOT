using AndroideIOT.Models;

namespace AndroideIOT.ViewModels;

public class AndroidChatViewModel
{
    public List<ChatMessage> Mensajes { get; } = new();
    public string TextoUsuario { get; set; } = string.Empty;
    public bool Cargando { get; set; }
    public bool EnviandoThingSpeak { get; set; }
    public bool? ThingSpeakOk { get; set; }
    public string? ErrorMensaje { get; set; }
    public AndroidMetrics Metricas { get; set; } = new();
}
