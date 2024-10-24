using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers;

[Authorize]
public class GenreController(IGenreService genreService) : Controller
{
  private readonly IGenreService _genreService = genreService;
  public IActionResult Add() => View();

  [HttpPost]
  public IActionResult Add(Genre model)
  {
    if (!ModelState.IsValid)
      return View(model);

    var result = _genreService.Add(model);
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

  public IActionResult Update(int id) => View(_genreService.GetById(id));
  // {
  //   var genre = _genreService.GetById(id);

  //   if (genre == null)
  //   {
  //     TempData["msg"] = $"Genre with ID {id} not found";
  //     return RedirectToAction(nameof(GetList));  // or return NotFound() if you prefer
  //   }

  //   return View(genre);
  // }

  [HttpPost]
  public IActionResult Update(Genre model)
  {
    if (!ModelState.IsValid)
      return View(model);

    var result = _genreService.Update(model);
    if (result)
    {
      TempData["msg"] = "Genre updated successfully";
      return RedirectToAction(nameof(GetList));
    }
    else
    {
      TempData["msg"] = "Error  on server side";
      return View(model);
    }
  }

  public IActionResult GetList() => View(_genreService.GetAll().ToList());

  public IActionResult Delete(int id)
  {
    _genreService.Delete(id);
    return RedirectToAction(nameof(GetList));
  }
}