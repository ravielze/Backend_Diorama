using Diorama.Internals.Resource;
using Diorama.Internals.Contract;
using Diorama.RestAPI.Repositories;
using Diorama.Internals.Responses;
using Diorama.Internals.Persistent.Models;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Diorama.RestAPI.Services;

public interface IUserService
{
    void Authenticate(AuthContract contract);
    void Register(RegisterAuthContract contract);
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

    public void Authenticate(AuthContract contract)
    {
        var user = _repo.Find(contract.Username);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Username not found.");
        }

        if (!_hasher.Verify(contract.Password, user.Password))
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Invalid password.");
        }
        var token = GenerateJWTToken(user);
        throw new ResponseOK(new UserAuthContract(user, token));
    }

    public void Register(RegisterAuthContract contract)
    {
        var user = _repo.Find(contract.Username);
        if (user != null)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Username is already registered.");
        }

        var registeredUser = new User(contract, _hasher);
        user = _repo.CreateNormalUser(registeredUser);
        throw new ResponseOK(new UserContract(user));
    }

    private string GenerateJWTToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (jwtSecret == null)
        {
            jwtSecret = "secreeetKeyDeliciousOnePapaUlalalala still need lebih panjang :(";
        }
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.ID.ToString()), new Claim("role", user.Role.Name) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}