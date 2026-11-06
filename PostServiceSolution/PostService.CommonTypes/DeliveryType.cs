using System.Text.Json.Serialization;

namespace PostService.CommonTypes;

[JsonConverter(typeof(JsonStringEnumConverter<DeliveryType>))]
public enum DeliveryType
{
    Department,
    Courier,
    ExpressCourier
}
