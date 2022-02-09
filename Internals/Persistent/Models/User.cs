using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;

[Index(nameof(Username), IsUnique = true)]
public class User : BaseEntity, IModel
{
    public int ID { get; set; } = 0;
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public string Biography { get; set; } = "";

    public int UserRoleID { get; set; } = 0;
    public UserRole Role { get; set; }
    public string ProfilePicture { get; set; } = "";
    public int Following { get; set; } = 0;
    public int Followers { get; set; } = 0;

    public User(UserRole role)
    {
        Role = role;
    }

    public User()
    {
        Role = new UserRole();
    }

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasOne(b => b.Role)
            .WithMany(r => r.Users)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
