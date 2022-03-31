using System;
using System.Net;
using System.Collections;
using Diorama.RestAPI.Repositories;
using Diorama.Internals.Contract;
using Diorama.Internals.Responses;
using Diorama.Internals.Persistent.Models;

namespace Diorama.RestAPI.Services;

public interface IPostService
{
    void CreatePost(int userId, CreatePostContract contract);
    void Comment(int userId, int postId, CommentContract contract);
    void GetPostForHomePage(int userId, int page);
    void GetPostForExplorePage(int userId, int page);
    void GetSpesificPost(int userId, int pageId);
    void LikePost(int userId, int pageId);
    void UnlikePost(int userId, int pageId);
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

    public void GetPostForExplorePage(int userId, int p)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        (var posts, var page, var maxPage) = _repo.GetNewestExplore(p);
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

    public void GetSpesificPost(int userId, int pageId)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        Post? post = _repo.FindById(pageId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        throw new ResponseOK(new PostContract(post));
    }

    public void LikePost(int userId, int pageId)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        Post? post = _repo.FindById(pageId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        PostLike newPostLikeInstance = new PostLike(user, post);
        try
        {
            _repo.Create(newPostLikeInstance);
        }
        catch (Exception)
        {
            throw new ResponseError(HttpStatusCode.BadRequest, "Can't like twice");
        }

        _repo.UpdateLike(post, "like");

        throw new ResponseOK("Like Success");
    }

    public void UnlikePost(int userId, int pageId)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        Post? post = _repo.FindById(pageId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        PostLike newPostLikeInstance = new PostLike(user, post);
        try
        {
            _repo.Delete(newPostLikeInstance);
        }
        catch (Exception)
        {
            throw new ResponseError(HttpStatusCode.BadRequest, "Can't unlike twice");
        }

        _repo.UpdateLike(post, "unlike");

        throw new ResponseOK("Unlike Success");
    }

    public void Comment(int userId, int postId, CommentContract contract)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }
        Post? post = _repo.FindById(postId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }
        Comment comment = new Comment();
        comment.Author = user;
        comment.AuthorID = user.ID;
        comment.Post = post;
        comment.PostID = post.ID;
        comment.Content = contract.Content;
        _repo.CreateComment(comment);
        throw new ResponseOK("Comment Success");
    }
}