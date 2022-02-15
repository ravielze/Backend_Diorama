using System.Net;

namespace Diorama.Internals.Responses;

public class ResponseOK : Exception
{
    public static HttpStatusCode StatusCode { get; } = HttpStatusCode.OK;
    public object Details { get; private set; } = "OK";

    public ResponseOK() : base(StatusCode.ToString())
    {
    }

    public ResponseOK(object details) : base(StatusCode.ToString() + ": " + details)
    {
        Details = details;
    }

}