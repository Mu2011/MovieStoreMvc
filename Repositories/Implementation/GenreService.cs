using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation;

public class GenreService(DatabaseContext context) : IGenreService
{
  private DatabaseContext _context = context;

  public bool Add(Genre model)
  {
    try
    {
      _context.Genre.Add(model);
      _context.SaveChanges();
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public bool Delete(int id)
  {
    try
    {
      if (GetById(id) is null)
        return false;

      _context.Genre.Remove(GetById(id)!);
      _context.SaveChanges();
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public IQueryable<Genre> GetAll() => _context.Genre.AsQueryable();

  public Genre? GetById(int id) => _context.Find<Genre>(id) is null ? throw new KeyNotFoundException() : _context.Find<Genre>(id);

  public bool Update(Genre model)
  {
    try
    {
      _context.Genre.Update(model);
      _context.SaveChanges();
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }
}