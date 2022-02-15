using BC = BCrypt.Net.BCrypt;

namespace Diorama.Internals.Resource;

public interface IHasher
{
    bool Verify(string rawPassword, string hashedPassword);
    string Hash(string hashedPassword);
}

public class BcryptHasher : IHasher
{
    public string Hash(string hashedPassword)
    {
        return BC.HashPassword(hashedPassword);
    }

    public bool Verify(string rawPassword, string hashedPassword)
    {
        return BC.Verify(rawPassword, hashedPassword);
    }
}