using Microsoft.AspNetCore.Mvc;
using Diorama.RestAPI.Services;
using Diorama.Internals.Contract;

namespace Diorama.RestAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPut("login")]
    public void Login(AuthContract contract)
    {
        _service.Authenticate(contract);
    }

    [HttpPut("register")]
    public void Register(RegisterAuthContract contract)
    {
        _service.Register(contract);
    }
}