using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IUserRepository
{
    void CreateNormalUser(User user);
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

    private void Create(User user, UserRole role)
    {
        dbContext.Entry(user.Role).State = EntityState.Unchanged;
        user.Role = role;
        db?.Add(user);
        Save();
    }

    public void CreateNormalUser(User user)
    {
        var normalUserRole = dbRole?.Find(1);
        if (normalUserRole == null)
        {
            throw new Exception("user role not found");
        }

        Create(user, normalUserRole);
    }

    public User? Find(string username)
    {
        return db?.Where(x => x.Username == username).First();
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