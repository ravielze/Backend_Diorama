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
        TimeSpan diff = DateTime.Now.Subtract(HealthDTO.StartTime);
        return new HealthDTO
        {
            Date = DateTime.Now,
            ServiceName = "Diorama-Backend",
            UpTime = String.Format("{0}h {1}m {2}s", diff.Hours, diff.Minutes, diff.Seconds)
        };
    }
}


public class HealthDTO
{
    public static readonly DateTime StartTime = DateTime.Now;
    public DateTime Date { get; set; }
    public String UpTime { get; set; } = "0:0:0.0";

    public string ServiceName { get; set; } = "unnamed";

}