using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class GalleryController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        return View();
    }
}