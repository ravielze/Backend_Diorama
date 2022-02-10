using System.Net;

namespace Diorama.Internals.Responses;

public class ResponseError : Exception
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    public object Details { get; set; } = "An unknown error has been occured";

    public ResponseError() : base(HttpStatusCode.InternalServerError.ToString() + ": An unknown error has been occured")
    {
    }

    public ResponseError(HttpStatusCode code, object details) : base(code.ToString() + ": " + details)
    {
        StatusCode = code;
        Details = details;
    }

}