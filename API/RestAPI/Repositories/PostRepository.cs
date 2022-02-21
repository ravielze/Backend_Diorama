using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IPostRepository
{
    Post Create(Post post);
    (IEnumerable<Post>, int, int) GetNewest(int requesterId, int page);
}

public class PostRepository : BaseRepository<Post>, IPostRepository
{

    public PostRepository(Database dbContext) : base(dbContext, dbContext.Post)
    {
    }

    public Post Create(Post post)
    {
        db!.Add(post);
        Save();
        return post;
    }

    public (IEnumerable<Post>, int, int) GetNewest(int requesterId, int page)
    {
        if (page < 1)
        {
            page = 1;
        }
        int offset = 20 * (page - 1);
        IEnumerable<int> following = dbContext.Follower!.
                                        Where(p => p.FollowSubjectID == requesterId).ToList().
                                        Select<Follower, int>(p => p.FollowObjectID);
        double count = db!.Count();
        int maxPage = 0;
        if (count > 0)
        {
            maxPage = (int)Math.Ceiling(count / 20);
        }
        return (db!.
            TemporalAll().
            Include(p => p.Author).
            Where(p => following.Contains(p.AuthorID)).
            OrderBy(e => EF.Property<DateTime>(e, "CreatedAt")).
            Take(20).
            Skip(offset).
            ToList(), page, maxPage);
    }
}