namespace VAAO_BE.Entities;

public class OpenAiChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public OpenAiUsage Usage { get; set; } = new();
}

public class OpenAiUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}
