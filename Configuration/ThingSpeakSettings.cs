namespace AndroideIOT.Configuration;

public class ThingSpeakSettings
{
    public string WriteApiKey { get; set; } = string.Empty;
    public string ReadApiKey { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.thingspeak.com";
}
