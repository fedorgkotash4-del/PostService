using PostService.CommonTypes;
using PostService.DataAccess;
using PostService.Models;

namespace PostService.BusinessLogic;

public class PostingService : IPostingService
{
    private readonly IPostingRepository _repository;

    public PostingService(IPostingRepository repository)
    {
        _repository = repository;
    }

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
        newPosting.CreatedAt = DateTime.UtcNow;
        newPosting.Id = _repository.Create(newPosting);
        return _repository.GetById(newPosting.Id)!;
    }

    public List<Posting> GetAll() => _repository.GetList();

    public Posting? Find(int postingId) => _repository.GetById(postingId);

    public Posting? Update(Posting posting)
    {
        if (_repository.GetById(posting.Id) is null) return null;
        Validate(posting);
        if (!float.IsFinite(posting.Price) || posting.Price < 0)
            throw new ArgumentException("Ціна має бути невід’ємною.");
        return _repository.Update(posting) == 0 ? null : _repository.GetById(posting.Id);
    }

    public int Delete(int postingId) => _repository.Delete(postingId);

    private static void Validate(Posting posting)
    {
        if (!float.IsFinite(posting.Weight) || posting.Weight <= 0)
            throw new ArgumentException("Вага має бути більшою за 0.");
        if (!Enum.IsDefined(posting.DeliveryType))
            throw new ArgumentException("Невідомий спосіб доставки.");
    }

}
