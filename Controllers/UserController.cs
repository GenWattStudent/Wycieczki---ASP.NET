using System.Security.Claims;
using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public UserController(ILogger<UserController> logger, IUserService userService, ITokenService tokenService)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
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
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userService.GetByUsername(registerModel.Username);

                if (userExists != null)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(registerModel);
                }

                await _userService.Register(registerModel);

                return RedirectToAction("Login");
            }

            return View(registerModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return View(registerModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginModel)
    {
        try
        {
            var user = await _userService.Login(loginModel);

            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password");
                return View(loginModel);
            }

            var token = _tokenService.Generate(user.Username, user.Role, user.Id, user.ImagePath, user.Contact?.Email);

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
            _logger.LogError(ex, "Error logging in user");
            return View(loginModel);
        }
    }

    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return RedirectToAction("Login");
    }

    async public Task<IActionResult> UserInfo()
    {
        var user = await _userService.GetByUsername(User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty);

        if (user == null)
        {
            return RedirectToAction("Logout");
        }

        var userViewModel = new UserViewModel(user);

        return View(userViewModel);
    }

    public async Task<IActionResult> EditUserInfo(UserViewModel userInfo)
    {
        Console.WriteLine("EditUserInfo " + userInfo.RegisterModel.Image);
        try
        {
            var user = await _userService.GetByUsername(User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty);

            if (user == null)
            {
                return RedirectToAction("Logout");
            }

            await _userService.EditUserInfo(userInfo.RegisterModel, user);

            return RedirectToAction("UserInfo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing user info");
            return BadRequest();
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
            _logger.LogError(ex, "Error deleting user");
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

            if (userExists != null)
            {
                return BadRequest("Admin already exists");
            }

            var user = new UserModel
            {
                Username = username,
                Password = password,
                Role = Role.Admin
            };

            await _userService.RegisterAdmin(user);

            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating admin");
            return BadRequest();
        }
    }

    public async Task<IActionResult> DeleteImage()
    {
        await _userService.DeleteImage(User.FindFirst(ClaimTypes.Name)?.Value);
        return RedirectToAction("UserInfo");
    }
}