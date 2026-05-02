using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;

namespace PayrollManagementSystem.Helpers;

public static class GroqHelper
{
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

    public static string Model { get; set; } = "llama-3.3-70b-versatile";

    public static async Task<string> AskAsync(
        string apiKey,
        string systemPrompt,
        IReadOnlyList<(string Role, string Text)> history)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Groq API key is not configured. Use the 'Configure AI' button.");

        var messages = new JsonArray();

        if (!string.IsNullOrEmpty(systemPrompt))
            messages.Add(new JsonObject { ["role"] = "system", ["content"] = systemPrompt });

        // Groq uses "assistant" instead of Gemini's "model"
        foreach (var (role, text) in history)
            messages.Add(new JsonObject
            {
                ["role"]    = role == "model" ? "assistant" : role,
                ["content"] = text
            });

        var body = new JsonObject
        {
            ["model"]       = Model,
            ["messages"]    = messages,
            ["temperature"] = 0.7,
            ["max_tokens"]  = 1024
        };

        var req = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        req.Headers.Add("Authorization", $"Bearer {apiKey}");
        req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(req);
        var json = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            var errMsg = JsonNode.Parse(json)?["error"]?["message"]?.GetValue<string>();
            throw new HttpRequestException(errMsg ?? $"Groq API error {(int)resp.StatusCode}");
        }

        return JsonNode.Parse(json)?["choices"]?[0]?["message"]?["content"]?.GetValue<string>()
               ?? "(No response from Groq)";
    }
}
