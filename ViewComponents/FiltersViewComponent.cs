using Book.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class FiltersViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new FilterModel();
        return View(model);
    }
}
