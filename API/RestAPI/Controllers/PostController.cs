using Microsoft.AspNetCore.Mvc;
using Diorama.Internals.Contract;
namespace Diorama.RestAPI.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;

    public PostController(ILogger<PostController> logger)
    {
        _logger = logger;
    }
}