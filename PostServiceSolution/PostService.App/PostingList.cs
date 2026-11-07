using System.Net.Http.Json;
using System.Text.Json;
using PostService.Dtos;

namespace PostService.App;

public partial class PostingList : Form
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("POSTSERVICE_API_URL")
            ?? "http://localhost:5036/"),
        Timeout = TimeSpan.FromSeconds(10)
    };
    private readonly CancellationTokenSource _closing = new();
    private bool _busy;
    private bool _resourcesReleased;

    public PostingList()
    {
        InitializeComponent();
        gridPostings.AutoGenerateColumns = false;
    }

    private async void PostingList_Load(object? sender, EventArgs e)
    {
        await LoadPostingsAsync();
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        await LoadPostingsAsync();
    }

    private async Task LoadPostingsAsync()
    {
        if (_busy || _closing.IsCancellationRequested) return;
        SetBusy(true);
        try
        {
            var postings = await _httpClient.GetFromJsonAsync<PostingGetDto[]>(
                "postings", _closing.Token) ?? [];
            if (_closing.IsCancellationRequested) return;
            gridPostings.DataSource = postings;
            lblStatus.Text = $"Відправлень: {postings.Length}";
        }
        catch (OperationCanceledException) when (_closing.IsCancellationRequested) { }
        catch (OperationCanceledException)
        {
            ShowError("Сервер не відповів вчасно. Спробуйте оновити список.");
        }
        catch (HttpRequestException)
        {
            ShowError("Не вдалося отримати дані з сервера. Перевірте, чи запущено PostService.");
        }
        catch (JsonException)
        {
            ShowError("Сервер повернув дані в неочікуваному форматі.");
        }
        finally
        {
            if (!_closing.IsCancellationRequested) SetBusy(false);
        }
    }

    private async void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_busy || _closing.IsCancellationRequested) return;
        if (gridPostings.CurrentRow?.DataBoundItem is not PostingGetDto selectedPosting)
        {
            MessageBox.Show(this, "Виберіть відправлення, яке потрібно видалити.",
                "Видалення відправлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SetBusy(true);
        try
        {
            using var response = await _httpClient.DeleteAsync(
                $"postings/{selectedPosting.Id}", _closing.Token);
            if (!_closing.IsCancellationRequested && !response.IsSuccessStatusCode)
                ShowError($"Не вдалося видалити відправлення. Сервер повернув статус {(int)response.StatusCode}.");
        }
        catch (OperationCanceledException) when (_closing.IsCancellationRequested) { }
        catch (OperationCanceledException)
        {
            ShowError("Сервер не відповів вчасно. Перевірте стан відправлення після оновлення списку.");
        }
        catch (HttpRequestException)
        {
            ShowError("Не вдалося видалити відправлення. Перевірте з’єднання із сервером.");
        }
        finally
        {
            if (!_closing.IsCancellationRequested) SetBusy(false);
        }
        // Навіть після 404 перечитуємо актуальний список із сервера.
        if (!_closing.IsCancellationRequested) await LoadPostingsAsync();
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        btnRefresh.Enabled = !busy;
        btnDelete.Enabled = !busy;
        gridPostings.Enabled = !busy;
        UseWaitCursor = busy;
    }

    private void ShowError(string message)
    {
        if (_closing.IsCancellationRequested) return;
        lblStatus.Text = "Не вдалося оновити дані";
        MessageBox.Show(this, message, "PostService", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ReleaseResources()
    {
        if (_resourcesReleased) return;
        _resourcesReleased = true;
        _closing.Cancel();
        _httpClient.Dispose();
        _closing.Dispose();
    }
}
