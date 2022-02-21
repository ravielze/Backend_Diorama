using Microsoft.AspNetCore.Mvc;
using Diorama.Internals.Contract;
using Diorama.Internals.Attributes;
using Diorama.RestAPI.Services;
namespace Diorama.RestAPI.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostService _service;

    public PostController(ILogger<PostController> logger, IPostService service)
    {
        _logger = logger;
        _service = service;
    }

    [Authorize]
    [HttpGet("personal")]
    public void GetHomePage([FromQuery(Name = "page")] int page = 1)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetPostForHomePage(userId, page);
    }

    [Authorize]
    [HttpGet("explore")]
    public void GetExplorePage([FromQuery(Name = "page")] int page = 1)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetPostForExplorePage(userId, page);
    }

    [Authorize]
    [HttpPost("{pageId}/like")]
    public void LikePost(int pageId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.LikePost(userId, pageId);
    }

    [Authorize]
    [HttpPost("{pageId}/unlike")]
    public void UnlikePost(int pageId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.UnlikePost(userId, pageId);
    }

    [Authorize]
    [HttpGet("{pageId}")]
    public void GetSpesificPost(int pageId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetSpesificPost(userId, pageId);
    }

    [Authorize]
    [HttpPost("")]
    public void CreatePost(CreatePostContract contract)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.CreatePost(userId, contract);
    }
}