using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;

public class Post : BaseEntitySoftDelete, IModel
{
    public int ID { get; set; } = 0;
    public int AuthorID { get; set; } = 0;

    [ForeignKey("AuthorID")]
    public User Author { get; set; } = new User();
    public string Caption { get; set; } = "";
    public string Image { get; set; } = "";
    public int Likes { get; set; } = 0;
    public IList<Comment> Comments { get; } = new List<Comment>();
    public IList<PostCategory> Categories { get; } = new List<PostCategory>();

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Post>()
            .HasQueryFilter(p => !p.DeletedAt.HasValue);
    }
}