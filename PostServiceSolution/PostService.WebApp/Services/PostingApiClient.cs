using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PostService.Dtos;

namespace PostService.WebApp.Services;

public sealed class PostingApiException(string message, bool notFound = false) : Exception(message)
{
    public bool NotFound { get; } = notFound;
}

public sealed class PostingApiClient(HttpClient client)
{
    public async Task<List<PostingGetDto>> GetAllAsync(CancellationToken token) =>
        await ReadAsync<List<PostingGetDto>>(HttpMethod.Get, "postings", null, token);

    public Task<PostingGetDto> GetAsync(int id, CancellationToken token) =>
        ReadAsync<PostingGetDto>(HttpMethod.Get, $"postings/{id}", null, token);

    public async Task CreateAsync(PostingPostDto dto, CancellationToken token) =>
        await SendAsync(HttpMethod.Post, "postings", JsonContent.Create(dto), token);

    public async Task UpdateAsync(int id, PostingPutDto dto, CancellationToken token) =>
        await SendAsync(HttpMethod.Put, $"postings/{id}", JsonContent.Create(dto), token);

    public async Task DeleteAsync(int id, CancellationToken token) =>
        await SendAsync(HttpMethod.Delete, $"postings/{id}", null, token);

    private async Task<T> ReadAsync<T>(HttpMethod method, string path, HttpContent? content, CancellationToken token)
    {
        using var response = await RequestAsync(method, path, content, token);
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(token)
                ?? throw new PostingApiException("Сервер повернув порожню відповідь. Спробуйте оновити сторінку.");
        }
        catch (JsonException) { throw new PostingApiException("Не вдалося прочитати відповідь сервера."); }
    }

    private async Task SendAsync(HttpMethod method, string path, HttpContent? content, CancellationToken token)
    {
        using var response = await RequestAsync(method, path, content, token);
    }

    private async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, HttpContent? content, CancellationToken token)
    {
        using var request = new HttpRequestMessage(method, path) { Content = content };
        HttpResponseMessage response;
        try { response = await client.SendAsync(request, token); }
        catch (HttpRequestException)
        { throw new PostingApiException("Немає з’єднання із PostService. Перевірте, чи запущено сервер, і спробуйте ще раз."); }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        { throw new PostingApiException("Сервер не відповів вчасно. Перевірте список перед повторним збереженням."); }
        if (response.IsSuccessStatusCode) return response;
        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new PostingApiException("Відправлення не знайдено. Можливо, його вже видалили. Поверніться до списку.", true);
            string message = $"Не вдалося виконати операцію (HTTP {(int)response.StatusCode}). Спробуйте пізніше.";
            try
            {
                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
                if (json.RootElement.ValueKind == JsonValueKind.Object && json.RootElement.TryGetProperty("error", out var error)
                    && error.ValueKind == JsonValueKind.String) message = error.GetString() ?? message;
            }
            catch (JsonException) { }
            throw new PostingApiException(message);
        }
    }
}
