using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;
public class PostCategory : IModel
{
    public int PostID { get; set; } = 0;
    public Post Post { get; set; } = new Post();

    public int CategoryID { get; set; } = 0;
    public Category Category { get; set; } = new Category();

    public PostCategory() {}

    public PostCategory(Post post, Category category) {
        Post = post;
        PostID = post.ID;

        Category = category;
        CategoryID = category.ID;
    }

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<PostCategory>()
            .HasKey(l => new { l.PostID, l.CategoryID });

        builder.Entity<PostCategory>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Categories)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PostCategory>()
            .HasOne(c => c.Category)
            .WithMany(p => p.PostCategories)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PostCategory>()
            .HasQueryFilter(c => !c.Post.DeletedAt.HasValue);
    }
}