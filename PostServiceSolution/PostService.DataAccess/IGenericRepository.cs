using PostService.Models;

namespace PostService.DataAccess;

public interface IGenericRepository<TModel> where TModel : ModelBase
{
    List<TModel> GetList();
    TModel? GetById(int id);
    int Create(TModel model);
    // Зберігаємо контракт ПР-41: кількість оновлених записів для PostingService.
    int Update(TModel model);
    int Delete(int id);
}
