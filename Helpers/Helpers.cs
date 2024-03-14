using Book.App.Repositories;
using Book.App.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book.App.Helpers;

public static class Helpers
{
    public static string IsActive(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "active")
    {
        string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
        string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

        IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
        IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

        return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
            cssClass : String.Empty;
    }

    public static string Truncate(this string value, int length)
    {
        if (value.Length <= length)
        {
            return value;
        }

        return value.Substring(0, length) + "...";
    }

    public static void AddMyServices(this IServiceCollection services)
    {
        services.AddTransient<TourService>();
        services.AddTransient<UserService>();
        services.AddTransient<TokenService>();
        services.AddTransient<BookService>();
        services.AddTransient<FileService>();
        services.AddTransient<WaypointService>();
        services.AddTransient<UserRepository>();
        services.AddTransient<GeoService>();
    }
}
