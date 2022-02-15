using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;
public class Comment : BaseEntitySoftDelete, IModel
{
    public int ID { get; set; } = 0;
    public int AuthorID { get; set; } = 0;

    [ForeignKey("AuthorID")]
    public User Author { get; set; } = new User();
    public int PostID { get; set; } = 0;
    public Post Post { get; set; } = new Post();
    public string Content { get; set; } = "";

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasQueryFilter(c => !c.DeletedAt.HasValue && !c.Post.DeletedAt.HasValue);
    }
}