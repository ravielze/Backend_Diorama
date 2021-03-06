using Diorama.Internals.Persistent;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface IUserRepository
{
    void EditUser(User user, string name, string username, string biography, string profilePicture);
    void CreateAdmin(User user);
    void UpdateFollowersFollowingTotal(User currentUser, User targetUser, string action);
    User CreateNormalUser(User user);
    User? Find(string username);
    User? FindById(int id);
    IEnumerable<User>? FindUsers(string username);
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

    public void EditUser(User user, string name, string username, string biography, string profilePicture)
    {
        user.Name = name;
        user.Username = username;
        user.Biography = biography;
        user.ProfilePicture = profilePicture;

        Save();
    }

    public User CreateNormalUser(User user)
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

    public IEnumerable<User>? FindUsers(string username)
    {
        return db?.Where(x => x.Username.ToLower().Contains(username.ToLower())).ToList();
    }

    public User? FindById(int id)
    {
        return db?.Where(x => x.ID == id).Include(x => x.Role).FirstOrDefault();
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

    public void UpdateFollowersFollowingTotal(User currentUser, User targetUser, string action)
    {
        int value;
        if (action == "follow")
        {
            value = 1;
        }
        else
        {
            value = -1;
        }

        currentUser.Following += value;
        Save();

        targetUser.Followers += value;
        Save();
    }
}