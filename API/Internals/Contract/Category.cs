using Diorama.Internals.Persistent.Models;
using System.ComponentModel.DataAnnotations;

namespace Diorama.Internals.Contract;

public class CategoriesContract
{
    public int Page { get; set; } = 1;
    public int MaxPage { get; set; } = 1;
    public IEnumerable<CategoryContract> Categories;

    public CategoriesContract(IEnumerable<Category> categories, int page, int maxPage)
    {
        Categories = categories.Select<Category, CategoryContract>(x => new CategoryContract(x));
        Page = page;
        MaxPage = maxPage;
    }
}

public class CategoryContract
{
    public int ID { get; set; } = 0;
    public string Name { get; set; } = "";

    public CategoryContract(Category category)
    {
        ID = category.ID;
        Name = category.Name;
    }
}