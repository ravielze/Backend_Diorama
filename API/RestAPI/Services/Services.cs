namespace Diorama.RestAPI.Services;

public class Services : ILayer
{
    private readonly IServiceCollection _services;

    public Services(IServiceCollection services)
    {
        _services = services;
    }

    public void Build()
    {
        _services.AddScoped<IUserService, UserService>();
    }
}