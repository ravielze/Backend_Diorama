using System;
using System.Net;
using System.Collections;
using Diorama.RestAPI.Repositories;
using Diorama.Internals.Contract;
using Diorama.Internals.Responses;
using Diorama.Internals.Persistent.Models;

namespace Diorama.RestAPI.Services;

public interface ICategoryService
{
    void GetCategoriesFromName(int userId, string name, int p);
}

public class CategoryService : ICategoryService
{
    private ICategoryRepository _repo;
    private IUserRepository _userRepo;

    public CategoryService(ICategoryRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public void GetCategoriesFromName(int userId, string name, int p)
    {
        User? user = _userRepo.FindById(userId);
        if (user == null)
        {
            throw new ResponseError(HttpStatusCode.Conflict, "Data inconsistent.");
        }

        (var categories, var page, var maxPage) = _repo.GetCategoriesByName(name, p);
        if (maxPage == 0)
        {
            throw new ResponseOK(new CategoriesContract(categories, 1, 1));
        }
        else if (page > maxPage)
        {
            throw new ResponseError(HttpStatusCode.NotFound, "Page not found.");
        }

        throw new ResponseOK(new CategoriesContract(categories, page, maxPage));
    }
}