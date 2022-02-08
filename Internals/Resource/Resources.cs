using Diorama.RestAPI;
namespace Diorama.Internals.Resource;

public class Resources : ILayer
{
    private readonly IServiceCollection _services;

    public Resources(IServiceCollection services)
    {
        _services = services;
    }

    public void Build()
    {
        _services.AddScoped<IHasher, BcryptHasher>();
    }
}