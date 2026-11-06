using PostService.Models;

namespace PostService.Services;

public class PostingService : IPostingService
{
    // Навчальне сховище спільне для scoped-екземплярів; дані зникають при перезапуску.
    private static readonly object Sync = new();
    private static readonly List<Posting> Postings = new()
    {
        new Posting { Id = 1, From = "Alice", To = "Bob", Content = "Books",
            DeliveryType = DeliveryType.Courier, Weight = 2.5f,
            Width = 30, Height = 20, Depth = 10, Value = 50, Price = 97.5f,
            CreatedAt = DateTime.UtcNow },
        new Posting { Id = 2, From = "Olena", To = "Andrii", Content = "Documents",
            DeliveryType = DeliveryType.Department, Weight = 0.5f,
            Width = 25, Height = 2, Depth = 18, Value = 100, Price = 45,
            CreatedAt = DateTime.UtcNow }
    };
    private static int nextId = 3;

    public Posting Create(Posting newPosting)
    {
        Validate(newPosting);
        // Формула навчальна: тарифів викладач не задав. Вага в кг, ціна в грн.
        // Department: 40 + 10 * вага; Courier: 60 + 15 * вага;
        // ExpressCourier: 100 + 25 * вага. Округлення до копійок.
        var (basePrice, perKg) = newPosting.DeliveryType switch
        {
            DeliveryType.Department => (40m, 10m),
            DeliveryType.Courier => (60m, 15m),
            DeliveryType.ExpressCourier => (100m, 25m),
            _ => throw new ArgumentException("Невідомий спосіб доставки.")
        };
        newPosting.Price = (float)Math.Round(basePrice + perKg * (decimal)newPosting.Weight, 2,
            MidpointRounding.AwayFromZero);
        lock (Sync)
        {
            newPosting.Id = nextId++;
            newPosting.CreatedAt = DateTime.UtcNow;
            Postings.Add(Copy(newPosting));
            return Copy(newPosting);
        }
    }

    public List<Posting> GetAll()
    {
        lock (Sync) { return Postings.Select(Copy).ToList(); }
    }

    public Posting? Find(int postingId)
    {
        lock (Sync)
        {
            var found = Postings.FirstOrDefault(p => p.Id == postingId);
            return found is null ? null : Copy(found);
        }
    }

    public Posting? Update(Posting posting)
    {
        lock (Sync)
        {
            var index = Postings.FindIndex(p => p.Id == posting.Id);
            if (index < 0) return null;
            Validate(posting);
            if (!float.IsFinite(posting.Price) || posting.Price < 0)
                throw new ArgumentException("Ціна має бути невід’ємною.");
            // Для PUT контракт містить Price. Автоматичний тариф застосовується при Create.
            posting.CreatedAt = Postings[index].CreatedAt;
            Postings[index] = Copy(posting);
            return Copy(posting);
        }
    }

    public int Delete(int postingId)
    {
        lock (Sync) { return Postings.RemoveAll(p => p.Id == postingId); }
    }

    private static void Validate(Posting posting)
    {
        if (!float.IsFinite(posting.Weight) || posting.Weight <= 0)
            throw new ArgumentException("Вага має бути більшою за 0.");
        if (!Enum.IsDefined(posting.DeliveryType))
            throw new ArgumentException("Невідомий спосіб доставки.");
    }

    private static Posting Copy(Posting p) => new()
    {
        Id = p.Id, From = p.From, To = p.To, Content = p.Content,
        DeliveryType = p.DeliveryType, Weight = p.Weight,
        Width = p.Width, Height = p.Height, Depth = p.Depth,
        Value = p.Value, Price = p.Price, CreatedAt = p.CreatedAt
    };
}
