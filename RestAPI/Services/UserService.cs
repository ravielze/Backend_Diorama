using Diorama.Internals.Resource;
using Diorama.Internals.Contract;
using Diorama.RestAPI.Repositories;
using Diorama.Internals.Persistent.Models;

namespace Diorama.RestAPI.Services;

public interface IUserService
{
    bool Authenticate(AuthContract contract);
    bool EditUserProfile(EditUserContract contract);
}

public class UserService : IUserService
{

    private IUserRepository _repo;
    private IHasher _hasher;

    public UserService(IUserRepository repo, IHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public bool Authenticate(AuthContract contract)
    {
        var user = _repo.Find(contract.Username);
        if (user == null)
        {
            return false;
        }

        if (!_hasher.Verify(contract.Password, user.Password))
        {
            return false;
        }

        return true;
    }

    public bool EditUserProfile(string username, EditUserContract contract) 
    {   
        var user = _repo.Find(username);
        if (user == null) 
        {
            return false;
        }

        if (username != contract.Username && _repo.Find(contract.Username) != null) 
        {
            return false;
        }

        _repo.EditUser(
            user, 
            contract.Name, 
            contract.Username, 
            contract.Biography, 
            contract.ProfilePicture,
        );
        return true;
    }

    public User? GetUserProfile(string username) 
    {
        var user = _repo.Find(username);

        return user;
    }
}