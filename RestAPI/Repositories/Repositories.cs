namespace Diorama.RestAPI.Repositories;

public class Repositories : ILayer
{
    private readonly IServiceCollection _services;

    public Repositories(IServiceCollection services)
    {
        _services = services;
    }

    public void Build()
    {
        //_services.AddScoped<ITestService, TestService>();
    }
}