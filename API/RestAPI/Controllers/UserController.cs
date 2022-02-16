using System;
using Microsoft.AspNetCore.Mvc;
using Diorama.RestAPI.Services;
using Diorama.Internals.Contract;
using Diorama.Internals.Attributes;

namespace Diorama.RestAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("login")]
    public void Login(AuthContract contract)
    {
        _service.Authenticate(contract);
    }

    [HttpPost("register")]
    public void Register(RegisterAuthContract contract)
    {
        _service.Register(contract);
    }

    [Authorize]
    [HttpGet("")]
    public void GetUserProfile()
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetUserProfile(userId);
    }

    [Authorize]
    [HttpPut("edit")]
    public void EditUserProfile(EditUserContract contract)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.EditUserProfile(userId, contract);
    }

    [Authorize]
    [HttpPost("{username}/follow")]
    public void Follow(string username)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.Follow(userId, username);
    }

    [Authorize]
    [HttpPost("{username}/unfollow")]
    public void Unfollow(string username)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.Unfollow(userId, username);
    }

    [Authorize]
    [HttpGet("{username}/is_following")]
    public void IsFollowing(string username)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.isFollowing(userId, username);
    }
}