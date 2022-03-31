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
    [HttpPost("{postId}/like")]
    public void LikePost(int postId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.LikePost(userId, postId);
    }

    [Authorize]
    [HttpPost("{postId}/unlike")]
    public void UnlikePost(int postId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.UnlikePost(userId, postId);
    }

    [Authorize]
    [HttpGet("{postId}")]
    public void GetSpesificPost(int postId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetSpesificPost(userId, postId);
    }

    [Authorize]
    [HttpPost("")]
    public void CreatePost(CreatePostContract contract)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.CreatePost(userId, contract);
    }

    [Authorize]
    [HttpPost("{postId}/comment")]
    public void CreateComment(int postId, CommentContract contract)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.Comment(userId, postId, contract);
    }
}