using System;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using Diorama.RestAPI.Repositories;
using Diorama.Internals.Contract;
using Diorama.Internals.Responses;
using Diorama.Internals.Persistent.Models;

namespace Diorama.RestAPI.Services;

public interface IPostService
{
    void Comment(int userId, int postId, CommentContract contract);
    void LikePost(int userId, int pageId);
    void UnlikePost(int userId, int pageId);
    void DeletePost(int userId, int postId);
    void GetSpesificPost(int userId, int pageId);
    void GetPostForHomePage(int userId, int p);
    void GetPostForExplorePage(int userId, int p);
    void EditPost(int userId, EditPostContract contract);
    void CreatePost(int userId, CreatePostContract contract);
    void GetCategoryPosts(int userId, int categoryId, int p);
    void GetPostComments(int postId);
}

public class PostService : IPostService
{
    private IPostRepository _repo;
    private IUserRepository _userRepo;
    private ICategoryRepository _categoryRepo;

    public PostService(IPostRepository repo, IUserRepository userRepo, ICategoryRepository categoryRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
        _categoryRepo = categoryRepo;
    }

    public void CreatePost(int userId, CreatePostContract contract)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        Post post = _repo.Create(new Post(user, contract));

        string caption = contract.Caption;
        string pattern = @"#(\w+)";

        Regex regex = new Regex(pattern);
        MatchCollection result = regex.Matches(caption);

        for (int i = 0; i < result.Count; i++)
        {
            string categoryName = result[i].Value.Remove(0, 1).ToLower();

            Category? category = _categoryRepo.FindByName(categoryName);
            if (category == null)
            {
                category = _categoryRepo.Create(new Category(categoryName));
            }

            _repo.Create(new PostCategory(post, category));
        }

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

    public void GetCategoryPosts(int userId, int categoryId, int p)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        (var posts, var page, var maxPage) = _repo.GetPostByCategory(categoryId, p);
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
    public void GetPostComments(int postId)
    {
        Post? post = _repo.FindById(postId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }
        var result = _repo.GetPostComments(postId);

        throw new ResponseOK(result.Select(x => new CommentResponseContract(x, x.Author.Username)));
    }

    public void DeletePost(int userId, int postId)
    {
        Post? post = _repo.FindById(postId);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }
        if (post.AuthorID != userId)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        _repo.DeletePost(post);

        throw new ResponseOK("Delete Success");
    }

    public void EditPost(int userId, EditPostContract contract)
    {
        Post? post = _repo.FindById(contract.ID);
        if (post == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        if (post.AuthorID != userId)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Post with spesific id not found.");
        }

        _repo.UpdatePost(post, contract.Caption);

        string caption = contract.Caption;
        string pattern = @"#(\w+)";

        Regex regex = new Regex(pattern);
        MatchCollection result = regex.Matches(caption);

        for (int i = 0; i < result.Count; i++)
        {
            string categoryName = result[i].Value.Remove(0, 1).ToLower();

            Category? category = _categoryRepo.FindByName(categoryName);
            if (category == null)
            {
                category = _categoryRepo.Create(new Category(categoryName));
            }

            _repo.Create(new PostCategory(post, category));
        }

        throw new ResponseOK("Edit Success");
    }
}