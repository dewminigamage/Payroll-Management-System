using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;

namespace PayrollManagementSystem.Helpers;

public static class GeminiHelper
{
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };
    public static string Model { get; set; } = "gemini-2.0-flash-lite";

    private static string ApiBase =>
        $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent";

    public static async Task<string[]> ListModelsAsync(string apiKey)
    {
        var resp = await _http.GetAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models?key={Uri.EscapeDataString(apiKey)}");
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            var err = JsonNode.Parse(json)?["error"]?["message"]?.GetValue<string>();
            throw new HttpRequestException(err ?? $"HTTP {(int)resp.StatusCode}");
        }
        var models = JsonNode.Parse(json)?["models"]?.AsArray();
        if (models == null) return [];
        return [.. models
            .Select(m => m?["name"]?.GetValue<string>() ?? "")
            .Where(n => n.StartsWith("models/gemini"))
            .Select(n => n.Replace("models/", ""))
            .Order()];
    }

    public static async Task<string> AskAsync(
        string apiKey,
        string systemPrompt,
        IReadOnlyList<(string Role, string Text)> history)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Gemini API key is not configured. Use the 'Configure Key' button.");

        // Inject system prompt into the first user message so it works with all API versions
        var contents = new JsonArray();
        bool injected = false;
        foreach (var (role, text) in history)
        {
            string messageText = text;
            if (!injected && role == "user")
            {
                messageText = systemPrompt + "\n\n" + text;
                injected = true;
            }
            contents.Add(new JsonObject
            {
                ["role"]  = role,
                ["parts"] = new JsonArray { new JsonObject { ["text"] = messageText } }
            });
        }

        var body = new JsonObject
        {
            ["contents"]         = contents,
            ["generationConfig"] = new JsonObject
            {
                ["temperature"]     = 0.7,
                ["maxOutputTokens"] = 1024
            }
        };

        var url     = $"{ApiBase}?key={Uri.EscapeDataString(apiKey)}";
        var payload = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        var resp    = await _http.PostAsync(url, payload);
        var json    = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            var errMsg = JsonNode.Parse(json)?["error"]?["message"]?.GetValue<string>()
                         ?? $"Gemini API error {(int)resp.StatusCode}";
            if (errMsg.Contains("not found", StringComparison.OrdinalIgnoreCase))
                errMsg += $"\n\nModel '{Model}' is unavailable for your key.\n"
                        + "→ Click 'Configure Key' → 'List Available Models' to see what works,\n"
                        + "  or get a fresh key from aistudio.google.com/app/apikey";
            throw new HttpRequestException(errMsg);
        }

        return JsonNode.Parse(json)?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
                       ?.GetValue<string>()
               ?? "(No response from Gemini)";
    }
}
