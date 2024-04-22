using System.Security.Claims;
using AutoMapper;
using Book.App.Filters.Exception;
using Book.App.Models;
using Book.App.Repositories;
using Book.App.Repositories.UnitOfWork;
using Book.App.Services;
using Book.App.Validators;
using Book.App.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Book.App.Helpers;

class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAgencyViewModel, TravelAgencyModel>().ReverseMap();
        CreateMap<RegisterViewModel, UserModel>();
        CreateMap<EditUserViewModel, UserModel>();
        CreateMap<EditAgencyViewModel, TravelAgencyModel>();
        CreateMap<AddTourViewModel, TourModel>().ReverseMap();
    }
}

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

    public static bool HasRole(this IHtmlHelper htmlHelper, string role)
    {
        var roles = htmlHelper.ViewContext.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value.Split(",");
        Console.WriteLine(htmlHelper.ViewContext.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value);
        foreach (var r in roles)
        {
            Console.WriteLine(r);
        }
        return roles != null && roles.Contains(role);
    }
    // get user id
    public static int GetCurrentUserId(this IHtmlHelper htmlHelper)
    {
        var userIdString = htmlHelper.ViewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdString != null ? int.Parse(userIdString) : 0;
    }

    public static int GetCurrentUserId(this ControllerBase controllerBase)
    {
        var userIdString = controllerBase.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdString != null ? int.Parse(userIdString) : 0;
    }

    public static int GetUserAgencyId(this ControllerBase controllerBase)
    {
        var agencyIdString = controllerBase.User.FindFirst("AgencyId")?.Value;
        return agencyIdString != null ? int.Parse(agencyIdString) : 0;
    }

    public static void AddMyServices(this IServiceCollection services)
    {
        const int MAX_BODY_SIZE = 104857600; // 100MB
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
        services.AddTransient<IAgencyFilesService, AgencyFilesService>();

        // validators
        services.AddTransient<IValidator<CreateAgencyViewModel>, TravelAgencyValidator>();
        services.AddTransient<IValidator<AddressModel>, AddressValidator>();
        services.AddTransient<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
        services.AddTransient<IValidator<ContactModel>, ContactValidator>();
        services.AddTransient<IValidator<EditUserViewModel>, EditUserViewModelValidator>();
        services.AddTransient<IValidator<LoginViewModel>, LoginViewModelValidator>();
        services.AddTransient<IValidator<AddTourViewModel>, AddTourViewModelValidator>();
        services.AddTransient<IValidator<WaypointModel>, EditWaypointValidator>();

        // Exceptions
        services.AddTransient<NotInAgencyExceptionFilter>();

        services.AddHttpContextAccessor();

        services.AddAutoMapper(typeof(MappingProfile));

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = MAX_BODY_SIZE;
        });
    }
}
