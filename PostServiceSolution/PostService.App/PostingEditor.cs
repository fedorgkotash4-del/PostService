using System.Net.Http.Json;
using System.Globalization;
using System.Text.Json;
using PostService.Dtos;
using PostService.CommonTypes;
namespace PostService.App;

public partial class PostingEditor : Form
{
    private HttpClient? _client;
    private int? _id;
    private bool _saving;
    private readonly CancellationTokenSource _closing = new();
    private readonly Dictionary<string, TextBox> _fields = new();
    private readonly Dictionary<string, Label> _messages = new();
    private ErrorProvider _errors = null!;
    private ComboBox _delivery = null!;
    private Label _summary = null!;
    private Button _save = null!;
    private Button _cancel = null!;

    public PostingEditor() { InitializeComponent(); }
    public PostingEditor(HttpClient client, int? id = null) : this()
    {
        _client = client; _id = id;
        Text = id.HasValue ? $"Редагування відправлення №{id}" : "Нове відправлення";
        _fields["Price"].ReadOnly = !id.HasValue;
        _fields["Price"].Text = id.HasValue ? "" : "Розраховується сервером";
        _save.Enabled = !id.HasValue;
        Shown += LoadRecord;
    }

    private async void LoadRecord(object? sender, EventArgs e)
    {
        if (!_id.HasValue || _client is null) return;
        try
        {
            using var response = await _client.GetAsync($"postings/{_id}", _closing.Token);
            if (!response.IsSuccessStatusCode) { ShowError(await ApiError.ReadAsync(response, _closing.Token)); return; }
            var dto = await response.Content.ReadFromJsonAsync<PostingGetDto>(_closing.Token);
            if (dto is null) { ShowError("Сервер повернув порожню відповідь."); return; }
            _fields["From"].Text = dto.From; _fields["To"].Text = dto.To; _fields["Content"].Text = dto.Content;
            _delivery.SelectedIndex = Array.IndexOf(Enum.GetValues<DeliveryType>(), dto.DeliveryType);
            foreach (var pair in new Dictionary<string,float> { ["Weight"]=dto.Weight, ["Width"]=dto.Width,
                ["Height"]=dto.Height, ["Depth"]=dto.Depth, ["Price"]=dto.Price })
                _fields[pair.Key].Text = pair.Value.ToString("G", CultureInfo.CurrentCulture);
            _fields["Value"].Text = dto.Value?.ToString("G", CultureInfo.CurrentCulture) ?? "";
            _fields["Description"].Text = dto.Description ?? "";
            _save.Enabled = true;
        }
        catch (OperationCanceledException) when (_closing.IsCancellationRequested) { }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or JsonException)
        { ShowError("Не вдалося завантажити відправлення. Перевірте з’єднання із сервером."); }
    }

    private async void SaveRecord(object? sender, EventArgs e)
    {
        if (_saving || _client is null) return;
        _errors.Clear(); foreach (var label in _messages.Values) label.Text = "";
        Control? first = null;
        void Error(string key, string message) {
            _errors.SetError(_fields[key], message); _messages[key].Text = message; first ??= _fields[key];
        }
        foreach (var key in new[] { "From", "To", "Content" })
            if (string.IsNullOrWhiteSpace(_fields[key].Text)) Error(key, "Заповніть обов’язкове поле.");
        var numbers = new Dictionary<string,float>();
        foreach (var key in new[] { "Weight", "Width", "Height", "Depth", "Value", "Price" })
        {
            if (key == "Price" && !_id.HasValue || key == "Value" && string.IsNullOrWhiteSpace(_fields[key].Text)) continue;
            bool positive = key is "Weight" or "Width" or "Height" or "Depth";
            if (!PostingInputValidator.TryNumber(_fields[key].Text, positive, out var number))
                Error(key, positive ? "Введіть число більше нуля." : "Введіть невід’ємне число.");
            else numbers[key] = number;
        }
        if (_delivery.SelectedIndex < 0) { first ??= _delivery; _errors.SetError(_delivery, "Виберіть тип доставки."); }
        if (first is not null) { _summary.ForeColor=Color.Firebrick; _summary.Text="Виправте позначені поля. Дані ще не збережено."; first.Focus(); return; }
        var dto = new PostingPostDto {
            From=_fields["From"].Text.Trim(), To=_fields["To"].Text.Trim(), Content=_fields["Content"].Text.Trim(),
            DeliveryType=Enum.GetValues<DeliveryType>()[_delivery.SelectedIndex], Weight=numbers["Weight"],
            Width=numbers["Width"], Height=numbers["Height"], Depth=numbers["Depth"],
            Value=numbers.TryGetValue("Value", out var value) ? value : null,
            Description=string.IsNullOrWhiteSpace(_fields["Description"].Text) ? null : _fields["Description"].Text.Trim()
        };
        _saving=true; _save.Enabled=false; _cancel.Enabled=false; ControlBox=false; UseWaitCursor=true;
        _summary.ForeColor=SystemColors.ControlText; _summary.Text="Збереження…";
        try
        {
            using var response = _id.HasValue
                ? await _client.PutAsJsonAsync($"postings/{_id}", new PostingPutDto {
                    Id=_id.Value, Price=numbers["Price"], From=dto.From, To=dto.To, Content=dto.Content,
                    DeliveryType=dto.DeliveryType, Weight=dto.Weight, Width=dto.Width, Height=dto.Height,
                    Depth=dto.Depth, Value=dto.Value, Description=dto.Description }, _closing.Token)
                : await _client.PostAsJsonAsync("postings", dto, _closing.Token);
            if (!response.IsSuccessStatusCode) { ShowError(await ApiError.ReadAsync(response, _closing.Token)); return; }
            DialogResult=DialogResult.OK;
        }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
        { ShowError("Не отримано підтвердження від сервера. Оновіть список перед повторною спробою збереження."); }
        finally { _saving=false; if (!IsDisposed) { _save.Enabled=true; _cancel.Enabled=true; ControlBox=true; UseWaitCursor=false; } }
    }
    private void ShowError(string message) { _summary.ForeColor=Color.Firebrick; _summary.Text=message; }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_saving && DialogResult != DialogResult.OK) e.Cancel=true;
        else _closing.Cancel();
        base.OnFormClosing(e);
    }
}
