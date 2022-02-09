namespace Diorama.RestAPI.Middleware;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class AuthHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public AuthHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        context.Items["user_id"] = null;
        context.Items["user_role"] = null;
        if (token != null)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
                if (jwtSecret == null)
                {
                    jwtSecret = "secreeetKeyDeliciousOnePapaUlalalala still need lebih panjang :(";
                }
                var key = Encoding.UTF8.GetBytes(jwtSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var role = jwtToken.Claims.First(x => x.Type == "role").Value;
                if (userId > 0)
                {
                    context.Items["user_id"] = userId;
                }
                if (role != null)
                {
                    context.Items["user_role"] = role;
                }
            }
            catch
            {
                // Ignore if failed.
            }
        }
        await _next(context);
    }
}