using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IUserRepository
{
    void CreateNormalUser(User user);
    void EditUser(User user, string name, string username, string biography, string profilePicture);
    void CreateAdmin(User user);
    User? Find(string username);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{

    // Subdomain
    protected DbSet<UserRole>? dbRole;

    public UserRepository(Database dbContext) : base(dbContext, dbContext.User)
    {
        dbRole = dbContext.UserRole;
    }

    private User Create(User user, UserRole role)
    {
        user.Role = role;
        db?.Add(user);
        Save();
        return user;
    }

    public void EditUser(User user, string name, string username, string biography, string profilePicture) {
        user.Name = name;
        user.Username = username;
        user.Biography = biography;
        user.ProfilePicture = profilePicture;

        Save();
    }

    public void CreateNormalUser(User user)
    {
        var normalUserRole = dbRole?.Find(1);
        if (normalUserRole == null)
        {
            throw new Exception("user role not found");
        }

        return Create(user, normalUserRole);
    }

    public User? Find(string username)
    {
        return db?.Where(x => x.Username == username).Include(x => x.Role).FirstOrDefault();
    }

    public void CreateAdmin(User user)
    {
        var normalUserRole = dbRole?.Find(2);
        if (normalUserRole == null)
        {
            throw new Exception("admin role not found");
        }

        Create(user, normalUserRole);
    }
}