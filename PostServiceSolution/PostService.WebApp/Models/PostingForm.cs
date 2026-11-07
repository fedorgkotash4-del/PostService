using System.ComponentModel.DataAnnotations;
using System.Globalization;
using PostService.CommonTypes;
using PostService.Dtos;

namespace PostService.WebApp.Models;

public sealed class PostingForm
{
    [Required(ErrorMessage = "Вкажіть відправника та адресу.")]
    public string From { get; set; } = "";
    [Required(ErrorMessage = "Вкажіть одержувача та адресу.")]
    public string To { get; set; } = "";
    [Required(ErrorMessage = "Вкажіть вміст відправлення.")]
    public string Content { get; set; } = "";
    public string DeliveryType { get; set; } = "Department";
    public string? Weight { get; set; }
    public string? Width { get; set; }
    public string? Height { get; set; }
    public string? Depth { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? Price { get; set; }

    public Dictionary<string, string> ValidateNumbers(bool editing)
    {
        var errors = new Dictionary<string, string>();
        foreach (var (name, text) in new[] { (nameof(Weight), Weight), (nameof(Width), Width),
            (nameof(Height), Height), (nameof(Depth), Depth) })
            if (!TryNumber(text, out var value) || value <= 0)
                errors[name] = "Введіть число більше нуля. Наприклад, 2,5.";
        if (!string.IsNullOrWhiteSpace(Value) && (!TryNumber(Value, out var declared) || declared < 0))
            errors[nameof(Value)] = "Вкажіть невід’ємне число або залиште поле порожнім.";
        if (editing && (!TryNumber(Price, out var price) || price < 0))
            errors[nameof(Price)] = "Введіть невід’ємну ціну доставки.";
        if (!Enum.TryParse<DeliveryType>(DeliveryType, out var type) || !Enum.IsDefined(type))
            errors[nameof(DeliveryType)] = "Виберіть один із запропонованих способів доставки.";
        // Розрахунок тарифу API використовує decimal, тому вага має вміщуватися в ньому.
        if (!editing && TryNumber(Weight, out var weight) && weight >= (float)(decimal.MaxValue / 25m))
            errors[nameof(Weight)] = "Значення ваги надто велике.";
        return errors;
    }
    public static bool TryNumber(string? text, out float number) =>
        float.TryParse(text?.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out number) && float.IsFinite(number);
    private static float Number(string? text) { TryNumber(text, out var value); return value; }

    public PostingPostDto ToPost() => new()
    {
        From = From.Trim(), To = To.Trim(), Content = Content.Trim(), DeliveryType = Enum.Parse<DeliveryType>(DeliveryType),
        Weight = Number(Weight), Width = Number(Width), Height = Number(Height), Depth = Number(Depth),
        Value = string.IsNullOrWhiteSpace(Value) ? null : Number(Value),
        Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim()
    };
    public PostingPutDto ToPut(int id)
    {
        var dto = ToPost();
        return new() { Id = id, Price = Number(Price), From = dto.From, To = dto.To, Content = dto.Content,
            DeliveryType = dto.DeliveryType, Weight = dto.Weight, Width = dto.Width, Height = dto.Height,
            Depth = dto.Depth, Value = dto.Value, Description = dto.Description };
    }
    public static PostingForm FromDto(PostingGetDto dto) => new()
    {
        From = dto.From, To = dto.To, Content = dto.Content, DeliveryType = dto.DeliveryType.ToString(),
        Weight = dto.Weight.ToString("G", CultureInfo.CurrentCulture), Width = dto.Width.ToString("G", CultureInfo.CurrentCulture),
        Height = dto.Height.ToString("G", CultureInfo.CurrentCulture), Depth = dto.Depth.ToString("G", CultureInfo.CurrentCulture),
        Value = dto.Value?.ToString("G", CultureInfo.CurrentCulture), Description = dto.Description,
        Price = dto.Price.ToString("G", CultureInfo.CurrentCulture)
    };
}
