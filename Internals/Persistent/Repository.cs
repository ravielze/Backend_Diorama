using Microsoft.EntityFrameworkCore;
namespace Diorama.Internals.Persistent;

public class Database : DbContext
{
    public Database(DbContextOptions<Database> options) : base(options)
    {
    }
}