using Microsoft.EntityFrameworkCore;
using Diorama.Internals.Persistent;

namespace Diorama.RestAPI.Repositories;

public class Repositories : ILayer
{
    private readonly IServiceCollection _services;

    public Repositories(IServiceCollection services)
    {
        _services = services;
    }

    public void Build()
    {
        _services.AddScoped<IUserRepository, UserRepository>();
    }
}

public class BaseRepository<TEntity> where TEntity : class
{

    protected DbSet<TEntity>? db;
    protected Database dbContext;

    public BaseRepository(Database dbContext, DbSet<TEntity>? dbSet)
    {
        this.db = dbSet;
        this.dbContext = dbContext;
    }

    public int Save()
    {
        return dbContext.SaveChanges();
    }
}