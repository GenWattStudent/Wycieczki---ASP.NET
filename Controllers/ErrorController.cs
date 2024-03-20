using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    TempData["ErrorMessage"] = "Sorry, the resource you requested could not be found";
                    return View("NotFound");

                case 401:
                    TempData["ErrorMessage"] = "Sorry, you are not authorized to access this resource";
                    return RedirectToAction("Login", "User");

                default:
                    TempData["ErrorMessage"] = "Sorry, something went wrong";
                    return View("ServerError");
            }
        }

        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                // Get which route the exception occurred at
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                // Get the exception that occurred
                Exception exceptionThatOccurred = exceptionFeature.Error;

                // Log your exception here
                _logger.LogError($"The path {routeWhereExceptionOccurred} threw an exception: {exceptionThatOccurred}");
            }

            return View("ServerError");
        }
    }
}