using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace Duren.EmailAutumn;

public class OpenAi
{
    private readonly ILogger<OpenAi> _logger;
    private readonly ChatClient _client;

    public OpenAi(ILogger<OpenAi> logger)
    {
        _logger = logger;
        var key =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
            ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY is not set");
        var endpoint =
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set");
        AzureOpenAIClient azureClient = new(new Uri(endpoint), new ApiKeyCredential(key));
        var clientName =
            Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT_NAME")
            ?? throw new InvalidOperationException("OPENAI_DEPLOYMENT_NAME is not set");
        _client = azureClient.GetChatClient(clientName);
    }
    

    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        try
        {
            ChatCompletion completion = await _client.CompleteChatAsync([prompt]);
            if (completion.Content.Count == 0)
            {
                throw new InvalidOperationException("Received empty response from OpenAI");
            }
            return completion.Content[0].Text;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while retrieving chat");
            throw;
        }
    }
}
