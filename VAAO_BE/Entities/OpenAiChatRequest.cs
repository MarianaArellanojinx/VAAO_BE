namespace VAAO_BE.Entities;

public class OpenAiChatRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? SystemPrompt { get; set; }
    public decimal? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public List<OpenAiChatMessage>? History { get; set; }
}

public class OpenAiChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
