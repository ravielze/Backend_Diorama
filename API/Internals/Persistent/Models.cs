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

public partial class Database : DbContext
{
    public DbSet<User>? User { get; set; }
    public DbSet<UserRole>? UserRole { get; set; }
    public DbSet<Post>? Post { get; set; }
    public DbSet<Follower>? Follower { get; set; }
    public DbSet<PostLike>? PostLike { get; set; }
}