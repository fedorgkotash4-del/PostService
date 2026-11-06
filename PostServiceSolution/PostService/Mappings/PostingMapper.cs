using PostService.Dtos;
using PostService.Models;

namespace PostService.Mappings;

public static class PostingMapper
{
    public static Posting ToPosting(PostingPostDto source) => new()
    {
        From = source.From,
        To = source.To,
        Content = source.Content,
        DeliveryType = source.DeliveryType,
        Weight = source.Weight,
        Width = source.Width,
        Height = source.Height,
        Depth = source.Depth,
        Value = source.Value
    };

    public static Posting ToPosting(PostingPutDto source) => new()
    {
        Id = source.Id,
        Price = source.Price,
        From = source.From,
        To = source.To,
        Content = source.Content,
        DeliveryType = source.DeliveryType,
        Weight = source.Weight,
        Width = source.Width,
        Height = source.Height,
        Depth = source.Depth,
        Value = source.Value
    };

    public static PostingGetDto ToPostingGetDto(Posting source) => new()
    {
        Id = source.Id,
        CreatedAt = source.CreatedAt,
        Price = source.Price,
        From = source.From,
        To = source.To,
        Content = source.Content,
        DeliveryType = source.DeliveryType,
        Weight = source.Weight,
        Width = source.Width,
        Height = source.Height,
        Depth = source.Depth,
        Value = source.Value
    };

}
