using Diorama.Internals.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Diorama.Internals.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private IList<string> _allowedRole = new List<string>();
    public AuthorizeAttribute() { }

    public AuthorizeAttribute(params string[] allowedRole)
    {
        foreach (var ar in allowedRole)
        {
            _allowedRole.Add(ar);
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = (int?)context.HttpContext.Items["user_id"];
        if (userId == null)
        {
            throw new ResponseError(HttpStatusCode.Unauthorized, "Not logged in.");
        }

        if (_allowedRole.Count() > 0)
        {
            var userRole = (string?)context.HttpContext.Items["user_role"];
            if (userRole == null || !_allowedRole.Contains(userRole))
            {
                throw new ResponseError(HttpStatusCode.Forbidden, "No permission.");
            }
        }
    }
}