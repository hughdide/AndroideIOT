namespace AndroideIOT.ViewModels;

public class ThingSpeakManualViewModel
{
    public int CargaCognitiva { get; set; }
    public int NivelCoherencia { get; set; }
    public int IntensidadEmocional { get; set; }
    public int LatenciaInferencia { get; set; }
    public int ConsumoEnergetico { get; set; }
    public bool Enviando { get; set; }
    public string MensajeEstado { get; set; } = string.Empty;
    public string AlertClass { get; set; } = "alert-success";
}
