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

public class LikeStatusContract
{
    public bool Status { get; set; }
    
    public LikeStatusContract(bool status)
    {
        Status = status;
    }
}

public class CommentContract
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = "";
}

public class CommentResponseContract
{
    public string Content { get; set; }
    public string From { get; set; }

    public CommentResponseContract(Comment c, String userName)
    {
        Content = c.Content;
        From = userName;
    }
}

public class EditPostContract
{

    [Required]
    public int ID { get; set; } = 0;

    [Required]
    [MinLength(1)]
    public string Caption { get; set; } = "";
}

public class PostsContract
{
    public int Page { get; set; } = 1;
    public int MaxPage { get; set; } = 1;
    public IEnumerable<PostContract> Posts;

    public PostsContract(IEnumerable<Post> posts, int page, int maxPage)
    {
        Posts = posts.Select<Post, PostContract>(x => new PostContract(x));
        Page = page;
        MaxPage = maxPage;
    }
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
}