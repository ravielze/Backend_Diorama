namespace Diorama.RestAPI.Middleware;

using System.Net;
using Diorama.Internals.Responses;
using Diorama.Internals.Contract;

public class ResponseHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            ResponseDetails result = new ResponseDetails();

            if (error is ResponseError responseError)
            {
                result.Change(responseError.StatusCode, responseError.Details);
            }
            else if (error is ResponseOK responseOK)
            {
                result.Change(ResponseOK.StatusCode, responseOK.Details);
            }
            else
            {
                var code = HttpStatusCode.InternalServerError;
                result.Change(code, "An unknown error has been occured");
                Console.WriteLine(error);
            }

            await response.WriteAsync(result.ToString());
        }
    }
}