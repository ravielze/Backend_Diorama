using Microsoft.AspNetCore.Mvc;
using Diorama.Internals.Contract;
namespace Diorama.RestAPI.Controllers;

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
    public HealthContract Get()
    {
        TimeSpan diff = DateTime.Now.Subtract(HealthContract.StartTime);
        return new HealthContract
        {
            Date = DateTime.Now,
            ServiceName = "Diorama-Backend",
            UpTime = String.Format("{0}h {1}m {2}s", diff.Hours, diff.Minutes, diff.Seconds)
        };
    }
}