using Diorama.Internals.Resource;
using Diorama.Internals.Contract;
using Diorama.RestAPI.Repositories;

namespace Diorama.RestAPI.Services;

public interface IUserService
{
    bool Authenticate(AuthContract contract);
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
}