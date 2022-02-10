using Diorama.Internals.NamingStrategy;
using System.Net;

namespace Diorama.Internals.Contract;

public class ResponseDetails
{
    public int StatusCode { get; private set; }
    public string StatusCodeDetails { get; private set; } = "";
    public object Details { get; private set; } = "";

    public ResponseDetails()
    {
        HttpStatusCode codeOk = HttpStatusCode.OK;
        SetStatusCode(codeOk);
        Details = new object();
    }

    public ResponseDetails(HttpStatusCode code)
    {
        SetStatusCode(code);
        Details = new object();
    }

    public ResponseDetails(HttpStatusCode code, object details)
    {
        SetStatusCode(code);
        Details = details;
    }

    public void Change(HttpStatusCode code)
    {
        SetStatusCode(code);
        Details = new object();
    }

    public void Change(HttpStatusCode code, object details)
    {
        SetStatusCode(code);
        Details = details;
    }

    private void SetStatusCode(HttpStatusCode code)
    {
        StatusCode = (int)code;
        StatusCodeDetails = code.ToString().ToSnakeCase();
    }

    public override string ToString()
    {
        return this.ToSnakeCase();
    }
}