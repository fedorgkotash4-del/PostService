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
        gridPostings.SelectionChanged += (_, _) => UpdateActions();
        gridPostings.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await EditAsync(); };
        gridPostings.CellFormatting += (_, e) => {
            if (e.ColumnIndex == colDeliveryType.Index && e.Value is PostService.CommonTypes.DeliveryType type) {
                e.Value=PostingInputValidator.DeliveryName(type); e.FormattingApplied=true;
            }
        };
        UpdateActions();
    }

    private async void PostingList_Load(object? sender, EventArgs e)
    {
        await LoadPostingsAsync(waitForStartup: true);
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        await LoadPostingsAsync();
    }

    private async Task<bool> LoadPostingsAsync(bool waitForStartup = false)
    {
        if (_busy || _closing.IsCancellationRequested) return false;
        SetBusy(true);
        try
        {
            PostingGetDto[] postings;
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    postings = await _httpClient.GetFromJsonAsync<PostingGetDto[]>("postings", _closing.Token) ?? [];
                    break;
                }
                catch (HttpRequestException ex) when (waitForStartup && ex.StatusCode is null && attempt < 19)
                {
                    lblStatus.Text = "Очікування запуску сервера…";
                    await Task.Delay(250, _closing.Token);
                }
            }
            if (_closing.IsCancellationRequested) return false;
            gridPostings.DataSource = postings;
            lblStatus.Text = $"Відправлень: {postings.Length}";
            return true;
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
        return false;
    }

    private async void btnCreate_Click(object? sender, EventArgs e)
    {
        if (_busy) return;
        using var editor=new PostingEditor(_httpClient);
        if (editor.ShowDialog(this)==DialogResult.OK) {
            bool loaded=await LoadPostingsAsync();
            if (!_closing.IsCancellationRequested) lblStatus.Text = loaded ? lblStatus.Text + " • Нове відправлення збережено" : "Збережено. Не вдалося оновити список — натисніть «Оновити».";
        }
    }
    private async void btnEdit_Click(object? sender, EventArgs e) => await EditAsync();
    private async Task EditAsync()
    {
        if (_busy || gridPostings.CurrentRow?.DataBoundItem is not PostingGetDto selected) return;
        using var editor=new PostingEditor(_httpClient, selected.Id);
        if (editor.ShowDialog(this)==DialogResult.OK) {
            bool loaded=await LoadPostingsAsync();
            if (_closing.IsCancellationRequested) return;
            foreach (DataGridViewRow row in gridPostings.Rows)
                if (row.DataBoundItem is PostingGetDto dto && dto.Id==selected.Id) gridPostings.CurrentCell=row.Cells[colFrom.Index];
            lblStatus.Text = loaded ? lblStatus.Text + " • Зміни збережено" : "Збережено. Не вдалося оновити список — натисніть «Оновити».";
        }
    }
    private void UpdateActions()
    {
        btnEdit.Enabled=btnDelete.Enabled=!_busy && gridPostings.CurrentRow?.DataBoundItem is PostingGetDto;
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

        if (MessageBox.Show(this, $"Видалити відправлення №{selectedPosting.Id}?\n{selectedPosting.From} → {selectedPosting.To}\nЦю дію не можна скасувати.",
            "Видалення відправлення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        SetBusy(true);
        try
        {
            using var response = await _httpClient.DeleteAsync(
                $"postings/{selectedPosting.Id}", _closing.Token);
            if (!_closing.IsCancellationRequested && !response.IsSuccessStatusCode)
                ShowError(await ApiError.ReadAsync(response, _closing.Token));
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
        btnCreate.Enabled = !busy;
        UpdateActions();
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
