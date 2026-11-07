using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DataAccess;

public class GenericEfRepository<TModel> : IGenericRepository<TModel>
    where TModel : ModelBase
{
    private readonly DbContext _context;

    public GenericEfRepository(DbContext context)
    {
        _context = context;
    }

    public List<TModel> GetList() => _context.Set<TModel>().OrderBy(m => m.Id).ToList();

    public TModel? GetById(int id) => _context.Set<TModel>().FirstOrDefault(m => m.Id == id);

    public int Create(TModel model)
    {
        _context.Set<TModel>().Add(model);
        _context.SaveChanges();
        return model.Id;
    }

    public int Update(TModel model)
    {
        var entity = _context.Set<TModel>().Find(model.Id);
        if (entity is null) return 0;
        // Сервіс уже міг прочитати запис у цьому scope. Оновлюємо саме
        // відстежуваний екземпляр, щоб не створити конфлікт двох об'єктів з одним Id.
        _context.Entry(entity).CurrentValues.SetValues(model);
        _context.Entry(entity).State = EntityState.Modified;
        var affected = _context.SaveChanges();
        // Повертаємо фактичні значення, включно з незмінюваними полями моделі.
        _context.Entry(entity).Reload();
        return affected;
    }

    public int Delete(int id)
    {
        var entity = _context.Set<TModel>().Find(id);
        if (entity is null) return 0;
        _context.Set<TModel>().Remove(entity);
        return _context.SaveChanges();
    }
}
