using Diorama.RestAPI.Repositories;
using Diorama.Internals.Contract;
using Diorama.Internals.Responses;
using Diorama.Internals.Persistent.Models;
using System.Net;

namespace Diorama.RestAPI.Services;

public interface IPostService
{
    void CreatePost(int userId, CreatePostContract contract);
    void GetPostForHomePage(int userId, int page);
}

public class PostService : IPostService
{

    private IPostRepository _repo;
    private IUserRepository _userRepo;

    public PostService(IPostRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public void CreatePost(int userId, CreatePostContract contract)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }
        _repo.Create(new Post(user, contract));
        throw new ResponseOK("Post created.");
    }

    public void GetPostForHomePage(int userId, int p)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        (var posts, var page, var maxPage) = _repo.GetNewest(user!.ID, p);
        if (maxPage == 0)
        {
            throw new ResponseOK(new PostsContract(posts, 1, 1));
        }
        else if (page > maxPage)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Page not found.");
        }

        throw new ResponseOK(new PostsContract(posts, page, maxPage));
    }
}