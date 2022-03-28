using Microsoft.AspNetCore.Mvc;
using Diorama.Internals.Contract;
using Diorama.Internals.Attributes;
using Diorama.RestAPI.Services;
namespace Diorama.RestAPI.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly ICategoryService _service;

    public CategoryController(ILogger<CategoryController> logger, ICategoryService service)
    {
        _logger = logger;
        _service = service;
    }

    [Authorize]
    [HttpGet("")]
    public void GetCategoriesFromName(
        [FromQuery(Name = "name")] string name = "", 
        [FromQuery(Name = "page")] int page = 1
    )
    {
        int userId = (int)HttpHelper.ContextItems["user_id"];
        _service.GetCategoriesFromName(userId, name, page);
    }
}