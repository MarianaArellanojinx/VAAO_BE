using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using VAAO_BE.Entities;
using VAAO_BE.Options;

namespace VAAO_BE.Services;

public class AzureOpenAiService : IAzureOpenAiService
{
    private const string DefaultDeploymentName = "gpt-5.2-chat";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly AzureOpenAiOptions _options;

    public AzureOpenAiService(HttpClient httpClient, IOptions<AzureOpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<OpenAiChatResponse> CreateChatCompletionAsync(OpenAiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            throw new ArgumentException("The prompt is required.", nameof(request));
        }

        ValidateConfiguration();

        var messages = new List<object>();
        var systemPrompt = string.IsNullOrWhiteSpace(request.SystemPrompt)
            ? _options.DefaultSystemPrompt
            : request.SystemPrompt;

        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            messages.Add(new { role = "system", content = systemPrompt });
        }

        if (request.History is not null)
        {
            messages.AddRange(
                request.History
                    .Where(message => !string.IsNullOrWhiteSpace(message.Role) && !string.IsNullOrWhiteSpace(message.Content))
                    .Select(message => new
                    {
                        role = message.Role.Trim().ToLowerInvariant(),
                        content = message.Content
                    }));
        }

        messages.Add(new { role = "user", content = request.Prompt });

        var payload = new Dictionary<string, object?>
        {
            ["messages"] = messages
        };

        if (request.MaxTokens.HasValue)
        {
            payload["max_completion_tokens"] = request.MaxTokens.Value;
        }

        if (request.Temperature.HasValue && request.Temperature.Value == 1)
        {
            payload["temperature"] = request.Temperature.Value;
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, BuildRequestUri());
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.Add("api-key", _options.ApiKey);
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Azure OpenAI request failed with status {(int)response.StatusCode}: {responseContent}");
            }

            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;
            var answer = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            var model = root.TryGetProperty("model", out var modelElement) ? modelElement.GetString() ?? string.Empty : string.Empty;

            var usage = new OpenAiUsage();
            if (root.TryGetProperty("usage", out var usageElement))
            {
                usage.PromptTokens = usageElement.TryGetProperty("prompt_tokens", out var promptTokens) ? promptTokens.GetInt32() : 0;
                usage.CompletionTokens = usageElement.TryGetProperty("completion_tokens", out var completionTokens) ? completionTokens.GetInt32() : 0;
                usage.TotalTokens = usageElement.TryGetProperty("total_tokens", out var totalTokens) ? totalTokens.GetInt32() : 0;
            }

            return new OpenAiChatResponse
            {
                Answer = answer,
                Model = model,
                Usage = usage
            };
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException)
        {
            throw new InvalidOperationException(
                $"No se pudo resolver el host de Azure OpenAI. Revisa AzureOpenAI:Endpoint en appsettings. Valor actual: {_options.Endpoint}",
                ex);
        }
    }

    private string BuildRequestUri()
    {
        var endpoint = _options.Endpoint.TrimEnd('/');
        var deploymentNameValue = string.IsNullOrWhiteSpace(_options.DeploymentName)
            ? DefaultDeploymentName
            : _options.DeploymentName;
        var deploymentName = Uri.EscapeDataString(deploymentNameValue);
        var apiVersion = Uri.EscapeDataString(_options.ApiVersion);

        return $"{endpoint}/openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured.");
        }

        if (!Uri.TryCreate(_options.Endpoint, UriKind.Absolute, out var endpointUri))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint no tiene un formato valido.");
        }

        if (!string.Equals(endpointUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint debe usar https.");
        }

        if (!endpointUri.Host.Contains("openai.azure.com", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"AzureOpenAI:Endpoint no parece ser valido. Debe terminar en openai.azure.com. Valor actual: {_options.Endpoint}");
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured.");
        }

        if (_options.Endpoint.Contains("tu-recurso.openai.azure.com", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint sigue con el placeholder y debe reemplazarse por tu recurso real.");
        }
    }
}
