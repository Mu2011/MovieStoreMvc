using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation;

public class MovieService(DatabaseContext context) : IMovieService
{
  readonly private DatabaseContext _context = context;

  public bool Add(Movie model)
  {
    try
    {
      _context.Movie.Add(model);
      _context.SaveChanges();
      foreach (int genreId in model.Genres)
      {
        var movieGenre = new MovieGenre
        {
          MovieId = model.Id,
          GenreId = genreId
        };
        _context.MovieGenre.Add(movieGenre);
      }
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
      var data = GetById(id);
      if (data is null)
        return false;

      var movieGenres = _context.MovieGenre.Where(a => a.MovieId == data.Id);

      foreach (var movieGenre in movieGenres)
      {
        _context.MovieGenre.Remove(movieGenre);
      }

      _context.Movie.Remove(data);
      _context.SaveChanges();
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public MovieListVm GetAll(string term = "", bool paging = false, int currentPage = 0) //=> new() { MovieList = _context.Movie.AsQueryable() };
  {
    var list = _context.Movie.ToList();
    var data = new MovieListVm();

    if (!string.IsNullOrEmpty(term))
    {
      term = term.ToLower();
      list = list.Where(x => x.Title.StartsWith(term, StringComparison.CurrentCultureIgnoreCase)).ToList();
    }

    if (paging)
    {
      // here we will apply paging
      int pageSize = 5;
      int count = list.Count;
      int TotalPage = (int)Math.Ceiling(count / (double)pageSize);
      list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

      data.PageSize = pageSize;
      data.CurrentPage = currentPage;
      data.TotalPages = TotalPage;
    }

    foreach (var movie in list)
    {
      var genres = (from genre in _context.Genre
                    join mg in _context.MovieGenre on genre.Id equals mg.GenreId
                    where mg.MovieId == movie.Id
                    select genre.GenreName).ToList();
      var genreName = string.Join(", ", genres);
      movie.GenreNames = genreName;
    }
    data.MovieList = list.AsQueryable();
    return data;
  }

  public Movie GetById(int id) => _context.Find<Movie>(id) ?? throw new KeyNotFoundException($"Movie with ID {id} was not found.");

  public List<int> GetGenreByMovieId(int movieId)
  {
    var genreIds = _context.MovieGenre.Where(x => x.MovieId == movieId).Select(x => x.GenreId).ToList();
    return genreIds;
  }

  public bool Update(Movie model)
  {
    try
    {
      // these genreIds are not selected by users and still present is movieGenre table corresponding to 
      // this movieId. So these ids should be removed.
      var genresToDelete = _context.MovieGenre.Where(x => x.MovieId == model.Id && !model.Genres.Contains(x.GenreId)).ToList();
      foreach (var mGenre in genresToDelete) _context.MovieGenre.Remove(mGenre);

      foreach (int genreId in model.Genres)
      {
        var movieGenre = _context.MovieGenre.Where(a => a.MovieId == model.Id && a.GenreId == genreId).FirstOrDefault();
        if (movieGenre is null)
        {
          movieGenre = new MovieGenre
          {
            MovieId = model.Id,
            GenreId = genreId
          };
          _context.MovieGenre.Add(movieGenre);
        }
      }
      _context.Movie.Update(model);
      // we have to add these Genre Ids in movie genre table

      _context.SaveChanges();
      return true;
    }
    catch (Exception)
    {
      return false;
    }
  }
}