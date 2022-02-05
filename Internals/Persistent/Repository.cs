using Microsoft.EntityFrameworkCore;
namespace Diorama.Internals.Persistent;

public class Repository : DbContext
{
    public Repository(DbContextOptions<Repository> options) : base(options)
    {
    }
}