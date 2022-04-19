using System;
using System.Collections;
using Diorama.Internals.Persistent;
using Diorama.Internals.Contract;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IPostRepository
{
    Post Create(Post post);

    Post? FindById(int id);

    PostLike Create(PostLike postLike);

    PostLike? GetPostLike(int userId, int postId);

    PostCategory Create(PostCategory postCategory);

    IEnumerable<Post> GetNewestExplore();
    Comment CreateComment(Comment comment);
    IEnumerable<Comment> GetPostComments(int postId);
    IEnumerable<Post> GetNewest(int requesterId);
    IEnumerable<Post> GetSelf(int requesterId);
    IEnumerable<Post> GetPostByCategory(int categoryId);

    void DeletePost(Post post);
    void Delete(PostLike postLike);
    void UpdateLike(Post post, string action);
    void UpdatePost(Post post, String caption);
}

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    protected DbSet<Comment> dbComment;

    public PostRepository(Database dbContext) : base(dbContext, dbContext.Post)
    {
        dbComment = dbContext.Comment!;
    }

    public PostCategory Create(PostCategory postCategory)
    {
        dbContext.PostCategory!.Add(postCategory);
        Save();
        return postCategory;
    }

    public PostLike Create(PostLike postLike)
    {
        dbContext.PostLike!.Add(postLike);
        Save();
        return postLike;
    }

    public void Delete(PostLike postLike)
    {
        dbContext.PostLike!.Remove(postLike);
        Save();
    }

    public PostLike? GetPostLike(int userId, int postId)
    {
        return dbContext.PostLike!.Where(x => x.UserID == userId && x.PostID == postId).FirstOrDefault();
    }

    public Post Create(Post post)
    {
        db!.Add(post);
        Save();
        return post;
    }

    public Comment CreateComment(Comment comment)
    {
        dbComment.Add(comment);
        Save();
        return comment;
    }

    public IEnumerable<Comment> GetPostComments(int postId)
    {
        return dbComment.Where(x => x.PostID == postId).Include(x => x.Author).ToList();
    }

    public void UpdateLike(Post post, string action)
    {
        int value;
        if (action == "like")
        {
            value = 1;
        }
        else
        {
            value = -1;
        }

        post.Likes += value;
        Save();
    }

    public Post? FindById(int id)
    {
        return db?.Where(x => x.ID == id).Include(p => p.Author).FirstOrDefault();
    }

    public void DeletePost(Post post)
    {
        IEnumerable<PostCategory> postCategoryList = dbContext
            .PostCategory!
            .Where(p => p.PostID == post.ID)
            .ToList();
        dbContext.PostCategory!.RemoveRange(postCategoryList);
        db?.Remove(post);
        Save();
    }

    public void UpdatePost(Post post, String caption)
    {
        IEnumerable<PostCategory> postCategoryList = dbContext
            .PostCategory!
            .Where(p => p.PostID == post.ID)
            .ToList();
        dbContext.PostCategory!.RemoveRange(postCategoryList);
        post.Caption = caption;
        Save();
    }

    public IEnumerable<Post> GetNewest(int requesterId)
    {
        IEnumerable<int> following = dbContext
            .Follower!
            .Where(p => p.FollowSubjectID == requesterId)
            .ToList()
            .Select<Follower, int>(p => p.FollowObjectID);

        return db!
                .TemporalAll()
                .Include(p => p.Author)
                .Where(p => following.Contains(p.AuthorID))
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(50)
                .ToList();
    }

    public IEnumerable<Post> GetSelf(int requesterId)
    {
        return db!
                .TemporalAll()
                .Include(p => p.Author)
                .Where(p => p.AuthorID == requesterId)
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .ToList();
    }

    public IEnumerable<Post> GetNewestExplore()
    {
        return db!
                .TemporalAll()
                .Include(p => p.Author)
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(50)
                .ToList();
    }

    public IEnumerable<Post> GetPostByCategory(int categoryId)
    {
        IEnumerable<int> postsId = dbContext
            .PostCategory!
            .Where(p => p.CategoryID == categoryId)
            .ToList()
            .Select<PostCategory, int>(p => p.PostID);
        return db!
                .TemporalAll()
                .Include(p => p.Author)
                .Where(p => postsId.Contains(p.ID))
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(50)
                .ToList();
    }
}