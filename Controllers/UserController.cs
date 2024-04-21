using System.Security.Claims;
using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterViewModel> _registerValidator;
    private readonly IValidator<EditUserViewModel> _editUserValidator;
    private readonly IValidator<LoginViewModel> _loginValidator;

    public UserController(
        IUserService userService,
        ITokenService tokenService,
        IValidator<RegisterViewModel> registerValidator,
        IValidator<EditUserViewModel> editUserValidator,
        IValidator<LoginViewModel> loginViewModel)
    {
        _userService = userService;
        _tokenService = tokenService;
        _registerValidator = registerValidator;
        _editUserValidator = editUserValidator;
        _loginValidator = loginViewModel;
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerModel)
    {
        try
        {
            var validationResult = await _registerValidator.ValidateAsync(registerModel);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, null);
                return View(registerModel);
            }

            await _userService.Register(registerModel);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error registering user";
            return View(registerModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginModel)
    {
        try
        {
            var validationResult = await _loginValidator.ValidateAsync(loginModel);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, null);
                return View(loginModel);
            }

            var user = await _userService.GetByUsername(loginModel.Username);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Error logging in";
                return View(loginModel);
            }

            var token = _tokenService.Generate(user.Username, user.Roles, user.Id, user.Contact.Email, user.TravelAgencyId ?? 0);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error logging in";
            return View(loginModel);
        }
    }

    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return RedirectToAction("Login");
    }

    [Authorize]
    async public Task<IActionResult> UserInfo()
    {
        var user = await _userService.GetByUsername(User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty);

        if (user == null) return RedirectToAction("Logout");

        var userViewModel = new UserViewModel(user);

        return View(userViewModel);
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> EditUserInfo(UserViewModel userInfo)
    {
        try
        {
            var user = await _userService.GetByUsername(User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty);
            var validationResult = await _editUserValidator.ValidateAsync(userInfo.EditUserViewModel);

            if (user == null) return RedirectToAction("Logout");

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, null);
                return View("UserInfo", new UserViewModel(user));
            }

            await _userService.EditUserInfo(userInfo.EditUserViewModel, user);

            return RedirectToAction("UserInfo");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error editing user info";
            return RedirectToAction("UserInfo");
        }
    }

    [Authorize]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            await _userService.Delete(username);
            return RedirectToAction("Logout");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting user";
            return BadRequest();
        }
    }

    public async Task<IActionResult> CreateAdmin()
    {
        var username = "admin";
        var password = "admin";

        try
        {
            var userExists = await _userService.GetByUsername(username);

            if (userExists != null) return BadRequest("Admin already exists");

            var user = new UserModel
            {
                Username = username,
                Password = password,
                Roles = new List<RoleModel> { new RoleModel { Role = Role.Admin } }
            };

            await _userService.RegisterAdmin(user);

            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error creating admin";
            return BadRequest();
        }
    }

    [Authorize]
    public async Task<IActionResult> DeleteImage()
    {
        await _userService.DeleteImage(User.FindFirst(ClaimTypes.Name)?.Value);
        return RedirectToAction("UserInfo");
    }
}