using Microsoft.EntityFrameworkCore;
namespace Diorama.Internals.Persistent;

public partial class Database : DbContext
{
    public readonly ILoggerFactory MyLoggerFactory;

    public Database(DbContextOptions<Database> options) : base(options)
    {
        MyLoggerFactory = LoggerFactory.Create(lf => lf.AddConsole());
    }

    private readonly ModelOptions _modelOptions = new ModelOptions();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _modelOptions.Configure(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);

        builder.UseLoggerFactory(MyLoggerFactory);
    }


    public override int SaveChanges()
    {
        OnBeforeSaving();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return (await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return (await base.SaveChangesAsync(cancellationToken));
    }

    protected void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        var utcNow = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.Entity is IBeforeSave bc)
            {
                var ok = bc.BeforeSave(bc);
                if (!ok)
                {
                    entry.State = EntityState.Detached;
                    continue;
                }
            }
            if (entry.Entity is BaseEntity trackable)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        trackable.UpdatedAt = utcNow;
                        entry.Property("CreatedAt").IsModified = false;
                        break;
                    case EntityState.Added:
                        trackable.UpdatedAt = utcNow;
                        trackable.CreatedAt = utcNow;
                        break;
                    case EntityState.Deleted:
                        if (entry.Entity is BaseEntitySoftDelete softDeleteTrackable)
                        {
                            entry.State = EntityState.Modified;
                            softDeleteTrackable.DeletedAt = utcNow;
                        }
                        break;
                }
            }
        }
    }
}