using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;

[Index(nameof(FollowObjectID), nameof(FollowSubjectID), Name = "ix_followings")]
public class Follower : IModel
{
    public int FollowSubjectID { get; set; } = 0;
    [ForeignKey("FollowSubjectID")]
    public User FollowSubject { get; set; } = new User();
    public int FollowObjectID { get; set; } = 0;

    [ForeignKey("FollowObjectID")]
    public User FollowObject { get; set; } = new User();

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<Follower>()
            .HasOne(f => f.FollowSubject)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Follower>()
            .HasOne(f => f.FollowObject)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Follower>()
            .HasKey(l => new { l.FollowSubjectID, l.FollowObjectID });
    }
}