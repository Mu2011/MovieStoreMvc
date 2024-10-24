using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers;

[Authorize]
public class MovieController(IMovieService movieService, IFileService fileService, IGenreService genreService) : Controller
{
  private readonly IMovieService _movieService = movieService;
  private readonly IFileService _fileService = fileService;
  private readonly IGenreService _genreService = genreService;

  public IActionResult Add() //=> View();
  {
    var model = new Movie
    {
      GenreList = _genreService.GetAll().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() })
    };
    return View(model);
  }
  [HttpPost]
  public IActionResult Add(Movie model)
  {
    model.GenreList = _genreService.GetAll().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
    if (!ModelState.IsValid)
      return View(model);

    if (model.ImageFile is not null)
    {
      var fileResult = _fileService.SaveImage(model.ImageFile);

      if (fileResult.Item1 == 0)
      {
        TempData["msg"] = "Could not save image";
        return View(model);
      }
      var imageName = fileResult.Item2;
      model.MovieImage = imageName;
    }

    var result = _movieService.Add(model);
    if (result)
    {
      TempData["msg"] = "Genre added successfully";
      return RedirectToAction(nameof(Add));
    }
    else
    {
      TempData["msg"] = "Error  on server side";
      return View(model);
    }
  }

  public IActionResult Edit(int id) //=> View(_movieService.GetById(id));
  {
    var model = _movieService.GetById(id);

    var selectGenres = _movieService.GetGenreByMovieId(model.Id);
    MultiSelectList multiSelectList = new(_genreService.GetAll(), "Id", "GenreName", selectGenres);
    model.MultiGenreList = multiSelectList;


    // if (model is null)
    // {
    //   TempData["msg"] = $"Movie with ID {id} not found";
    //   return RedirectToAction(nameof(MovieList));  // or return NotFound() if you prefer
    // }

    return View(model);
  }
  [HttpPost]
  public IActionResult Edit(Movie model)
  {
    var selectGenres = _movieService.GetGenreByMovieId(model.Id);
    MultiSelectList multiSelectList = new(_genreService.GetAll(), "Id", "GenreName", selectGenres);
    model.MultiGenreList = multiSelectList;

    if (!ModelState.IsValid)
      return View(model);

    if (model.ImageFile is not null)
    {
      var fileResult = _fileService.SaveImage(model.ImageFile);

      if (fileResult.Item1 == 0)
      {
        TempData["msg"] = "Could not save image";
        return View(model);  // or return RedirectToAction() with error message if you prefer
      }
      var imageName = fileResult.Item2;
      model.MovieImage = imageName;
    }

    var result = _movieService.Update(model);
    if (result)
    {
      TempData["msg"] = "Movie updated successfully";
      return RedirectToAction(nameof(MovieList));
    }
    else
    {
      TempData["msg"] = "Error  on server side";
      return View(model);
    }
  }

  public IActionResult MovieList() => View(_movieService.GetAll());

  public IActionResult Delete(int id)
  {
    _movieService.Delete(id);
    return RedirectToAction(nameof(MovieList));
  }
}