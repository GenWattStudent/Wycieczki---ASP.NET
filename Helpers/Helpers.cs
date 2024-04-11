using Book.App.Models;
using Book.App.Repositories;
using Book.App.Repositories.UnitOfWork;
using Book.App.Services;
using Book.App.Validators;
using FluentValidation;
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
        // Repositories
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<ITourRepository, TourRepository>();
        services.AddTransient<IImageRepository, ImageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<ITourService, TourService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IReservationService, ReservationService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IWaypointService, WaypointService>();
        services.AddTransient<IGeoService, GeoService>();
        services.AddTransient<IWeatherService, WeatherService>();
        services.AddTransient<IMealService, MealService>();
        services.AddTransient<IAgencyService, AgencyService>();

        // validators
        services.AddTransient<IValidator<TravelAgencyModel>, TravelAgencyValidator>();
    }
}
