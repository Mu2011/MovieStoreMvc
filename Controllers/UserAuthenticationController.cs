using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers;

public class UserAuthenticationController(IUserAuthenticationService authService) : Controller
{
  private readonly IUserAuthenticationService _authService = authService;
  /* We will Create a user with admin rights, after that we are going to comment this method because we need only one user in this application,
    If you need other users, you can create implement this registration method with view 
    I have create a complete tutorial for this, you can check the link in description box
  */

  // public async Task<IActionResult> Register()
  // {
  //   var model = new RegistrationModel
  //   {
  //     Email = "admin@gmail.com",
  //     Username = "admin",
  //     Name = "Mahmoud",
  //     Password = "Admin123@",
  //     PasswordConfirm = "Admin123@",
  //     Role = "Admin"
  //   };
  //   // If you want to register with user, Change Role = "User"

  //   var result = await _authService.RegisterAsync(model);
  //   return Ok(result.Message);
  // }

  public Task<IActionResult> Login() => Task.FromResult<IActionResult>(View());

  [HttpPost]
  public async Task<IActionResult> Login(LoginModel model)
  {
    if (!ModelState.IsValid)
      return View(model);

    var result = await _authService.LoginAsync(model);
    if (result.StatusCode == 200)
      return RedirectToAction("Index", "Home");
    else
    {
      TempData["msg"] = "Could not logged in...";
      return RedirectToAction(nameof(Login));
    }
  }

  public async Task<IActionResult> Logout()
  {
    await _authService.LogoutAsync();
    return RedirectToAction(nameof(Login));
  }
}