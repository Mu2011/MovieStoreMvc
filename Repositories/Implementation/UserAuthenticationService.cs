using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation;

public class UserAuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
  SignInManager<ApplicationUser> signInManager) : IUserAuthenticationService
{
  private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

  private readonly RoleManager<IdentityRole> _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));

  private readonly SignInManager<ApplicationUser> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));

  // public Task<Status> ChangePasswordAsync(ChangePasswordModel model, string newPassword)
  // {
  //   var status = new Status();
  //   var user = await _userManager.FindByNameAsync(model.Username);
  //   if (user is null)
  //   {
  //     status.StatusCode = 401;
  //     status.Message = "Invalid username or password";
  //     return status;
  //   }
  //   if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
  //   {
  //     status.StatusCode = 401;
  //     status.Message = "Invalid current password";
  //     return status;
  //   }

  //   var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, newPassword);
  //   if (result.Succeeded)
  //   {
  //     status.StatusCode = 200;
  //     status.Message = "Password changed successfully";
  //   }
  //   else
  //   {
  //     foreach (var error in result.Errors)
  //       status.Messages.Add(error.Description);
  //   }

  //   return status;
  // }

  public async Task<Status> LoginAsync(LoginModel model)
  {
    var status = new Status();
    var user = await _userManager.FindByNameAsync(model.Username);

    if (user is null)
    {
      status.StatusCode = 401;
      status.Message = "Invalid username or password";
      return status;
    }
    if (!await _userManager.CheckPasswordAsync(user, model.Password))
    {
      status.StatusCode = 401;
      status.Message = "Invalid username or password";
      return status;
    }

    var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
    if (signInResult.Succeeded)
    {
      var userRoles = await _userManager.GetRolesAsync(user);
      var authClaims = new List<Claim>
      {
        new(ClaimTypes.Name, user.UserName!)
      };
      foreach (var role in userRoles)
        authClaims.Add(new Claim(ClaimTypes.Role, role));

      // var authProperties = new AuthenticationProperties
      // {
      //   IsPersistent = true,
      //   ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
      // };
      // await _signInManager.SignInAsync(user, authProperties);
      status.StatusCode = 200;
      status.Message = "User logged in successfully";
    }
    else if (signInResult.IsLockedOut)
    {
      status.StatusCode = 401;
      status.Message = "User is locked out";
    }
    else
    {
      status.StatusCode = 401;
      status.Message = "Invalid username or password";
    }

    return status;
  }

  public async Task LogoutAsync() => await _signInManager.SignOutAsync();

  public async Task<Status> RegisterAsync(RegistrationModel model)
  {
    var status = new Status();
    var userExist = await _userManager.FindByNameAsync(model.Username!);
    if (userExist is not null)
    {
      status.StatusCode = 400;
      status.Message = "Username already exists";
      return status;
    }

    var user = new ApplicationUser()
    {
      Email = model.Email,
      SecurityStamp = Guid.NewGuid().ToString(),
      UserName = model.Username,
      Name = model.Name,
      EmailConfirmed = true,
      PhoneNumberConfirmed = true
    };

    var result = await _userManager.CreateAsync(user, model.Password!);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
        status.Message += $"{error.Description}\n";
      status.StatusCode = 400;
      return status;
    }

    if (!await _roleManager.RoleExistsAsync(model.Role!))
      await _roleManager.CreateAsync(new IdentityRole(model.Role!));
    if (await roleManager.RoleExistsAsync(model.Role!))
      await userManager.AddToRoleAsync(user, model.Role!);

    // if (model.Role is not null)
    // {
    //   if (await _roleManager.RoleExistsAsync(model.Role))
    //   {
    //     var addToRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
    //     if (!addToRoleResult.Succeeded)
    //     {
    //       foreach (var error in addToRoleResult.Errors)
    //         status.Message += $"{error.Description}\n";
    //       status.StatusCode = 400;
    //       return status;
    //     }
    //   }
    //   else
    //   {
    //     status.StatusCode = 400;
    //     status.Message = "Role does not exist";
    //     return status;
    //   }
    // }

    await _signInManager.SignInAsync(user, isPersistent: false);
    status.StatusCode = 200;
    status.Message = "You have registered successfully";
    return status;
  }
}