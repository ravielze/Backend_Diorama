using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IFollowerRepository
{
    void CreateFollower(Follower data);
    void DeleteFollower(Follower data);
    Follower? Find(int subjectId, int objectId);
}

public class FollowerRepository : BaseRepository<Follower>, IFollowerRepository
{

    public FollowerRepository(Database dbContext) : base(dbContext, dbContext.Follower) { }

    public void CreateFollower(Follower follower)
    {
        db?.Add(follower);
        Save();
    }

    public void DeleteFollower(Follower follower)
    {
        db?.Remove(follower);
        Save();
    }

    public Follower? Find(int subjectId, int objectId)
    {
        return db?
            .Where(x => x.FollowSubjectID == subjectId && x.FollowObjectID == objectId)
            .Include(x => x.FollowObject).Include(x => x.FollowSubject)
            .FirstOrDefault();
    }
}