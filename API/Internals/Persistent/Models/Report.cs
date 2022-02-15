using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;
public class Report : BaseEntitySoftDelete, IModel
{
    public int ID { get; set; } = 0;
    public int AuthorID { get; set; } = 0;

    [ForeignKey("AuthorID")]
    public User Author { get; set; } = new User();
    public int? PostID { get; set; }
    public Post? Post { get; set; }

    public int? CommentID { get; set; }
    public Comment? Comment { get; set; }
    public string Content { get; set; } = "";

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<Report>()
            .HasOne(r => r.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(r => r.Post)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(r => r.Comment)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}