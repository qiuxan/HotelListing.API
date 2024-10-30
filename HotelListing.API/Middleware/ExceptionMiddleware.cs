using HotelListing.API.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace HotelListing.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;   
        _logger = logger;
    }

    public async  Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something Went Wrong while processing {context.Request.Path}");
            context.Response.StatusCode = 500;
            await HandlerExceptionAsync(context, ex);
        }
    }

    private Task HandlerExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        var errorDetails = new ErrorDetails
        {
            ErrorMessage = ex.Message,
            ErrorType = "Failure"
        };

        switch (ex) 
        { 
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorDetails.ErrorType = "Not Found";
                break;
            default:
                break;
        }

        string response = JsonConvert.SerializeObject(errorDetails);
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(response);

    }

    public class ErrorDetails
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
    }
}
