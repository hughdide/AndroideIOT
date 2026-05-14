namespace AndroideIOT.Configuration;

public class AiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "llama3";
    public int MaxTokens { get; set; } = 1024;
    public string BaseUrl { get; set; } = "http://localhost:11434";
}
