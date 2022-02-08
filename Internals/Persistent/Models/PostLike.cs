using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;
public class PostLike : IModel
{
    public int UserID { get; set; } = 0;
    public User User { get; set; } = new User();

    public int PostID { get; set; } = 0;
    public Post Post { get; set; } = new Post();

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<PostLike>()
            .HasKey(l => new { l.UserID, l.PostID });

        builder.Entity<PostLike>()
            .HasQueryFilter(l => !l.Post.DeletedAt.HasValue);

        builder.Entity<PostLike>()
            .HasOne(p => p.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PostLike>()
            .HasOne(p => p.Post)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}