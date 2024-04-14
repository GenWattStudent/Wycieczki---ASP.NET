using System.Security.Claims;
using AutoMapper;
using Book.App.Models;
using Book.App.Repositories;
using Book.App.Repositories.UnitOfWork;
using Book.App.Services;
using Book.App.Validators;
using Book.App.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Book.App.Helpers;

class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAgencyViewModel, TravelAgencyModel>();
        CreateMap<TravelAgencyModel, CreateAgencyViewModel>();
        CreateMap<RegisterViewModel, UserModel>();
        CreateMap<EditUserViewModel, UserModel>();
        CreateMap<EditAgencyViewModel, TravelAgencyModel>();
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
        return roles != null && roles.Contains(role);
    }

    public static void AddMyServices(this IServiceCollection services)
    {
        const int MAX_BODY_SIZE = 104857600 / 2; // 50MB
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

        services.AddHttpContextAccessor();

        services.AddAutoMapper(typeof(MappingProfile));

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = MAX_BODY_SIZE;
        });
    }
}
