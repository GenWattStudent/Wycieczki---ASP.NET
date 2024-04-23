using Book.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class FiltersViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(FilterModel filter)
    {
        return View(filter);
    }
}
