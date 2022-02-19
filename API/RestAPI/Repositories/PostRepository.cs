using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IPostRepository
{
}

public class PostRepository : BaseRepository<Post>, IPostRepository
{

    public PostRepository(Database dbContext) : base(dbContext, dbContext.Post)
    {
    }
}