using Diorama.Internals.Contract;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category : IModel
{
    public int ID { get; set; } = 0;
    public string Name { get; set; } = "";
    public IList<PostCategory> PostCategories { get; } = new List<PostCategory>();

    public IList<Post> Posts
    {
        get
        {
            IList<Post> result = new List<Post>();
            foreach (var pc in PostCategories)
            {
                if (!pc.Post.DeletedAt.HasValue)
                {
                    result.Add(pc.Post);
                }
            }
            return result;
        }
    }

    public Category()
    {
    }

    public Category(string name)
    {
        Name = name;
    }

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<Category>();
    }
}