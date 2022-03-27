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
}