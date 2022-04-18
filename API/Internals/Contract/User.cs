using System.ComponentModel.DataAnnotations;
using Diorama.Internals.Persistent.Models;

namespace Diorama.Internals.Contract;

public class AuthContract
{

    [Required]
    [MinLength(4)]
    [MaxLength(32)]
    [RegularExpression(@"^(?=[a-zA-Z0-9._]{4,32}$)(?!.*[_.]{2})[^_.].*[^_.]$")]
    public string Username { get; set; } = "";

    [Required]
    [MinLength(8)]
    [MaxLength(60)]
    public string Password { get; set; } = "";

}

public class EditUserContract
{
    [Required]
    [MinLength(4)]
    [MaxLength(32)]
    [RegularExpression(@"^(?=[a-zA-Z0-9._]{4,32}$)(?!.*[_.]{2})[^_.].*[^_.]$")]
    public string Username { get; set; } = "";

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Biography { get; set; } = "";

    [Required]
    public string ProfilePicture { get; set; } = "";
}

public class RegisterAuthContract : AuthContract
{

    [Required]
    [MinLength(4)]
    [MaxLength(256)]
    public string Name { get; set; } = "";
}

public class UserContract
{
    public int ID { get; private set; }
    public string Username { get; private set; }
    public string Name { get; private set; }
    public string Biography { get; private set; }
    public string Role { get; private set; }
    public string ProfilePicture { get; private set; }
    public int Following { get; private set; }
    public int Followers { get; private set; }

    public UserContract(User user)
    {
        ID = user.ID;
        Username = user.Username;
        Name = user.Name;
        Biography = user.Biography;
        Role = user.Role.Name;
        ProfilePicture = user.ProfilePicture;
        Following = user.Following;
        Followers = user.Followers;
    }

}

public class MinUserContract
{
    public int ID { get; private set; }
    public string Username { get; private set; }
    public string Name { get; private set; }
    public string ProfilePicture { get; private set; }
    public MinUserContract(User user)
    {
        ID = user.ID;
        Username = user.Username;
        Name = user.Name;
        ProfilePicture = user.ProfilePicture;
    }
}

public class SearchUserContract
{
    public IEnumerable<MinUserContract> Users;

    public SearchUserContract(IEnumerable<User> users)
    {
        Users = users.Select<User, MinUserContract>(u => new MinUserContract(u));
    }
}

public class UserAuthContract
{
    public UserContract User { get; private set; }
    public string Token { get; private set; }

    public UserAuthContract(User user, string token)
    {
        User = new UserContract(user);
        Token = token;
    }
}