using PostService.CommonTypes;
namespace PostService.WebApp.Models;
public static class Display
{
    public static string Delivery(DeliveryType type) => type switch {
        DeliveryType.Department => "До відділення", DeliveryType.Courier => "Кур’єром",
        DeliveryType.ExpressCourier => "Експрес-кур’єром", _ => "Невідомий спосіб" };
}
