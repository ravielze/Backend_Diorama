using Microsoft.AspNetCore.Mvc;

namespace Diorama.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public HealthDTO Get()
    {
        return new HealthDTO
        {
            Date = DateTime.Now,
            ServiceName = "Diorama-Backend"
        };
    }
}

public class HealthDTO
{
    public DateTime Date { get; set; }

    public string ServiceName { get; set; } = "unnamed";

}