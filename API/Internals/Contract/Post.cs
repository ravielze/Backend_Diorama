using System.ComponentModel.DataAnnotations;
using Diorama.Internals.Persistent.Models;

namespace Diorama.Internals.Contract;

public class CreatePostContract
{

    [Required]
    [MinLength(1)]
    public string Caption { get; set; } = "";

    [Required]
    [MinLength(8)]
    [MaxLength(4096)]
    public string Image { get; set; } = "";

}

public class PostContract
{
    public int ID { get; set; } = 0;
    public string Caption { get; set; } = "";
    public string Image { get; set; } = "";
    public int Likes { get; set; } = 0;
    public UserContract Author { get; set; }

    public PostContract(Post post)
    {
        Author = new UserContract(post.Author);
        ID = post.ID;
        Caption = post.Caption;
        Image = post.Image;
        Likes = post.Likes;
    }

    public static IEnumerable<PostContract> Transform(IEnumerable<Post> posts)
    {
        return posts.Select<Post, PostContract>(p => new PostContract(p));
    }
}