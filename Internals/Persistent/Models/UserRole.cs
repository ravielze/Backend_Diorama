using Microsoft.EntityFrameworkCore;

namespace Diorama.Internals.Persistent.Models;

[Index(nameof(Name), IsUnique = true)]
public class UserRole : IModel
{
    public int ID { get; set; } = 0;
    public string Name { get; set; } = "";
    public IList<User> Users { get; } = new List<User>();

    public void Configure(ModelBuilder builder)
    {
        builder.Entity<UserRole>()
            .HasData(
                new UserRole { ID = 1, Name = "User" },
                new UserRole { ID = 2, Name = "Admin" }
            );
    }
}