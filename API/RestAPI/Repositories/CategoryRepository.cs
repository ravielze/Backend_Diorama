using System;
using System.Collections;
using Diorama.Internals.Persistent;
using Diorama.Internals.Contract;
using Diorama.Internals.Persistent.Models;
using Microsoft.EntityFrameworkCore;

namespace Diorama.RestAPI.Repositories;

public interface ICategoryRepository
{
    Category Create(Category category);

    Category? FindByName(string categoryName);

    (IEnumerable<Category>, int, int) GetCategoriesByName(string name, int page);

    void Delete(Category category);    
}

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(Database dbContext) : base(dbContext, dbContext.Category)
    {
    }

    public void Delete(Category category)
    {
        db!.Remove(category);
        Save();
    }

    public Category Create(Category category)
    {
        db!.Add(category);
        Save();
        return category;
    }

    public Category? FindByName(string categoryName)
    {
        return db?.Where(x => x.Name == categoryName).FirstOrDefault();
    }

    public (IEnumerable<Category>, int, int) GetCategoriesByName(string name, int page)
    {
        if (page < 1)
        {
            page = 1;
        }

        int offset = 20 * (page - 1);
        double count = db!.Where(e => EF.Functions.Like(e.Name, $"%{name}%")).Count();
        int maxPage = 0;

        if (count > 0)
        {
            maxPage = (int)Math.Ceiling(count / 20);
        }

        return (
            db!
                .TemporalAll()
                .Where(e => EF.Functions.Like(e.Name, $"%{name}%"))
                .Take(20)
                .Skip(offset)
                .ToList(),
            page, 
            maxPage
        );
    }
}