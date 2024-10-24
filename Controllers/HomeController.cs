using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers;

public class HomeController(IMovieService movieService) : Controller
{
  private readonly IMovieService _movieService = movieService;
  public IActionResult Index(string term = "", int currentPage = 1) //=> View();
  {
    var movieList = _movieService.GetAll(term, true, currentPage);
    return View(movieList);
  }

  public IActionResult About() => View();

  public IActionResult MovieDetail(int movieId) //=> View(_movieService.GetById(movieId));
  {
    var movie = _movieService.GetById(movieId);
    return View(movie);
  }
}