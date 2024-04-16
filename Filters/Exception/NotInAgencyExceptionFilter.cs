using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Book.App.Filters.Exception
{
    public class NotInAgencyException : System.Exception
    {
        public NotInAgencyException(string message = "You dont belong to this agency!") : base(message)
        {
        }
    }

    public class NotInAgencyExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is NotInAgencyException)
            {
                var httpContext = context.HttpContext;
                httpContext.Items["ErrorMessage"] = context.Exception.Message;
                // Perform redirection
                httpContext.Response.Redirect($"/Home/Index");

                // Set the response result (optional)
                context.Result = new EmptyResult();
                context.ExceptionHandled = true;
            }
        }
    }
}