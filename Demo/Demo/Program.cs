using System.ClientModel;
using Azure.AI.OpenAI;
using Demo.Enums;
using Microsoft.Extensions.AI;

IChatClient client = BuildChatClient(ModelMode.Offline);
List<ChatMessage> chatHistory = [];
var chatOptions = new ChatOptions
{
   Temperature = 2f,
};

// Show banner once at start
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("======================================");
Console.WriteLine("         AI Chat Console v1.2     ");
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
    switch(modelMode)
    {
        case ModelMode.Online:
            var endpoint = new Uri(""); 
            var apiKey = new ApiKeyCredential("");
            var deploymentName = "gpt-4o";
            var azureClient = new AzureOpenAIClient(endpoint, apiKey);
            return azureClient.GetChatClient(deploymentName).AsIChatClient();
        case ModelMode.Offline:
            return new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1:8b");
        default:
            throw new NotSupportedException($"Model mode {modelMode} is not supported.");
    }

}
