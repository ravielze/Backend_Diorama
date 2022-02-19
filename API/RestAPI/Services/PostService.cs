using Diorama.RestAPI.Repositories;

namespace Diorama.RestAPI.Services;

public interface IPostService
{
}

public class PostService : IPostService
{

    private IPostRepository _repo;

    public PostService(IPostRepository repo)
    {
        _repo = repo;
    }
}