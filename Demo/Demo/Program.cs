using System.ClientModel;
using Azure.AI.OpenAI;
using Demo.Enums;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

IChatClient client = BuildChatClient(ModelMode.Offline);
List<ChatMessage> chatHistory = [];

var mpcTools = await GetMcpToolsFromEShop();

var chatOptions = new ChatOptions
{
    Tools =
    [
        ..mpcTools
    ]
};

// Show banner once at start
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("======================================");
Console.WriteLine("         AI Chat Console v1.5     ");
Console.WriteLine("======================================");
Console.ResetColor();
Console.WriteLine("  Type /exit to quit\n");

while (true)
{
    // === USER PROMPT ===
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n┌──────────────────────────────── User Prompt ─────────────────────────────────────┐");
    Console.Write(" > ");
    Console.ResetColor();

    var userPrompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userPrompt))
        continue;

    if (userPrompt.Equals("/exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Exiting chat...");
        Console.ResetColor();
        break;
    }

    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // === AI RESPONSE ===
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("└──────────────────────────────────────────────────────────────────────────────────┘");
    Console.WriteLine($"AI Response [{DateTime.Now:HH:mm:ss}]");
    Console.ResetColor();

    var response = string.Empty;
    Console.ForegroundColor = ConsoleColor.Yellow;

    await foreach (var item in client.GetStreamingResponseAsync(chatHistory, chatOptions))
    {
        Console.Write(item.Text);
        response += item.Text;
    }

    Console.ResetColor();
    Console.WriteLine("\n────────────────────────────────────");

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
}

static IChatClient BuildChatClient(ModelMode modelMode)
{
    switch (modelMode)
    {
        case ModelMode.Online:
            var endpoint = new Uri("");
            var apiKey = new ApiKeyCredential("");
            var deploymentName = "gpt-4o";
            var azureClient = new AzureOpenAIClient(endpoint, apiKey);
            return azureClient.GetChatClient(deploymentName).AsIChatClient().AsBuilder().UseFunctionInvocation().Build();
        case ModelMode.Offline:
            return new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1:8b").AsBuilder().UseFunctionInvocation().Build();
        default:
            throw new NotSupportedException($"Model mode {modelMode} is not supported.");
    }
}

static async Task<IEnumerable<McpClientTool>>  GetMcpToolsFromEShop()
{
    var urlToRemoteServer = "http://localhost:5064";
    var clientTransport = new HttpClientTransport(
        new HttpClientTransportOptions
        {
            Name = "EShopRemoteServer",
            TransportMode = HttpTransportMode.StreamableHttp,
            Endpoint = new Uri(urlToRemoteServer),
        });
    McpClient mcpClient = await McpClient.CreateAsync(clientTransport);

    return await mcpClient.ListToolsAsync();
}