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
    [HttpGet("mine")]
    public void GetMine()
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetPostMine(userId);
    }

    [Authorize]
    [HttpGet("mine/{username}")]
    public void GetMineOther(string username)
    {
        _service.GetOtherPost(username);
    }

    [Authorize]
    [HttpGet("homepage")]
    public void GetHomePage()
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetPostForHomePage(userId);
    }

    [Authorize]
    [HttpGet("explore")]
    public void GetExplorePage([FromQuery(Name = "page")] int page = 1)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetPostForExplorePage(userId, page);
    }

    [Authorize]
    [HttpGet("{postId}/like/status")]
    public void GetLikeStatus(int postId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetLikeStatus(userId, postId);
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
    [HttpGet("category/{categoryId}")]
    public void GetCategoryPosts(int categoryId, [FromQuery(Name = "page")] int page = 1)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetCategoryPosts(userId, categoryId, page);
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

    [HttpGet("{postId}/comment")]
    public void Comments(int postId)
    {
        _service.GetPostComments(postId);
    }

    [HttpDelete("{postId}")]
    public void DeletePost(int postId)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.DeletePost(userId, postId);
    }

    [Authorize]
    [HttpPut("")]
    public void EditPost(EditPostContract contract)
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.EditPost(userId, contract);
    }
}