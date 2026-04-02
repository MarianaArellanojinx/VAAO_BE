namespace VAAO_BE.Options;

public class AzureOpenAiOptions
{
    public const string SectionName = "AzureOpenAI";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2024-10-21";
    public string DefaultSystemPrompt { get; set; } = "You are a helpful assistant.";
}
