using System.Net;
using System.Text.Json;
namespace PostService.App;
internal static class ApiError
{
    public static async Task<string> ReadAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (response.StatusCode == HttpStatusCode.NotFound) return "Відправлення вже видалено або не знайдено. Оновіть список.";
        try
        {
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
            foreach (var key in new[] { "error", "detail", "title" })
                if (json.RootElement.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
                    return value.GetString() ?? "Помилка сервера.";
        }
        catch (JsonException) { }
        return $"Сервер відхилив операцію (HTTP {(int)response.StatusCode}).";
    }
}
