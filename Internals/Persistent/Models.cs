using Microsoft.EntityFrameworkCore;
using Diorama.Internals.Persistent.Models;

namespace Diorama.Internals.Persistent;

public interface IModel
{
    void Configure(ModelBuilder builder);
}

public class ModelOptions
{
    private readonly IModel[] _modelList = new IModel[]{
        new User(), new UserRole(),
        new Post(), new PostLike(), new Comment(), new Report(),
        new Follower(), new PostCategory(), new Category()
    };

    public void Configure(ModelBuilder builder)
    {
        foreach (var model in _modelList)
        {
            model.Configure(builder);
        }
    }
}

public class ModelWrappers
{
    public DbSet<Follower>? Followers { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Post>? Post { get; set; }
}