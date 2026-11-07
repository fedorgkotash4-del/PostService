using System.Globalization;
using PostService.CommonTypes;
namespace PostService.App;

internal static class PostingInputValidator
{
    public static bool TryNumber(string text, bool positive, out float value)
    {
        return float.TryParse(text.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out value) && float.IsFinite(value) && (positive ? value > 0 : value >= 0);
    }
    public static string DeliveryName(DeliveryType type) => type switch
    {
        DeliveryType.Department => "У відділення",
        DeliveryType.Courier => "Кур’єрська",
        DeliveryType.ExpressCourier => "Експрес-кур’єрська",
        _ => "Невідомий тип"
    };
}
