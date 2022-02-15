using Diorama.Internals.NamingStrategy;

namespace Diorama.RestAPI.Controllers;

public class Controllers : ILayer
{
    private readonly IServiceCollection _services;

    public Controllers(IServiceCollection services)
    {
        _services = services;
    }

    public void Build()
    {
        _services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
            }
        );

        _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        var httpContextAccessor = _services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
        HttpHelper.Configure(httpContextAccessor);
    }
}

public static class HttpHelper
{
    private static IHttpContextAccessor? _httpContextAccessor;


    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public static Microsoft.AspNetCore.Http.HttpContext Current
    {
        get => _httpContextAccessor!.HttpContext!;
    }

    public static IDictionary<object, object> ContextItems
    {
        get => _httpContextAccessor!.HttpContext!.Items!;
    }
}