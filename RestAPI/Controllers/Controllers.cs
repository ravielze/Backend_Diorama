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
    }
}