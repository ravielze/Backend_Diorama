using System.ComponentModel.DataAnnotations;

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