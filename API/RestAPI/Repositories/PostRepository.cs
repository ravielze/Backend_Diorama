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

    PostCategory Create(PostCategory postCategory);

    (IEnumerable<Post>, int, int) GetNewestExplore(int page);
    (IEnumerable<Post>, int, int) GetNewest(int requesterId, int page);

    void DeletePost(Post post);
    void Delete(PostLike postLike);
    void UpdateLike(Post post, string action);
    void UpdatePost(Post post, String caption);
}

public class PostRepository : BaseRepository<Post>, IPostRepository
{

    public PostRepository(Database dbContext) : base(dbContext, dbContext.Post)
    {
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

    public Post Create(Post post)
    {
        db!.Add(post);
        Save();
        return post;
    }

    public void UpdateLike(Post post, string action)
    {
        int value;
        if(action == "like") {
            value = 1;
        } else {
            value = -1;
        }
        
        post.Likes += value;
        Save();
    }

    public Post? FindById(int id)
    {
        return db?.Where(x => x.ID == id).Include(p => p.Author).FirstOrDefault();
    }

    public void DeletePost(Post post){
        IEnumerable<PostCategory> postCategoryList = dbContext
            .PostCategory!
            .Where(p => p.PostID == post.ID)
            .ToList();
        dbContext.PostCategory!.RemoveRange(postCategoryList);
        db?.Remove(post);
        Save();
    }

    public void UpdatePost(Post post, String caption) {
        IEnumerable<PostCategory> postCategoryList = dbContext
            .PostCategory!
            .Where(p => p.PostID == post.ID)
            .ToList();
        dbContext.PostCategory!.RemoveRange(postCategoryList);
        post.Caption = caption;
        Save();
    }

    public (IEnumerable<Post>, int, int) GetNewest(int requesterId, int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        int offset = 20 * (page - 1);

        IEnumerable<int> following = dbContext
            .Follower!
            .Where(p => p.FollowSubjectID == requesterId)
            .ToList()
            .Select<Follower, int>(p => p.FollowObjectID);

        int maxPage = 0;
        double count = db!.Count();
        if (count > 0)
        {
            maxPage = (int)Math.Ceiling(count / 20);
        }

        return(
            db!
                .TemporalAll()
                .Include(p => p.Author)
                .Where(p => following.Contains(p.AuthorID))
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(20)
                .Skip(offset)
                .ToList(),
            page,
            maxPage
        );
    }

    public (IEnumerable<Post>, int, int) GetNewestExplore(int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        int offset = 20 * (page - 1);
        double count = db!.Count();
        int maxPage = 0;

        if (count > 0)
        {
            maxPage = (int)Math.Ceiling(count / 20);
        }

        return (
            db!
                .TemporalAll()
                .Include(p => p.Author)
                .OrderBy(e => EF.Property<DateTime>(e, "CreatedAt"))
                .Take(20)
                .Skip(offset)
                .ToList(), 
            page, 
            maxPage
        );
    }
}