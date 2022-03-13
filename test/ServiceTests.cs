using Diorama.Internals.Resource;
using Diorama.Internals.Responses;
using Diorama.RestAPI.Repositories;
using Moq;
using System;
using Xunit;

namespace DioramaTest;

public class MockWrappers
{
    public Mock<IPostRepository> PostRepo = new Mock<IPostRepository>();
    public Mock<IUserRepository> UserRepo = new Mock<IUserRepository>();
    public Mock<IFollowerRepository> FollowerRepo = new Mock<IFollowerRepository>();
    public Mock<IHasher> Hasher = new Mock<IHasher>();
}

public abstract class Tester
{

    protected ResponseOK IsOK(Action act)
    {
        return Assert.Throws<ResponseOK>(act);
    }

    protected ResponseError IsError(Action act)
    {
        return Assert.Throws<ResponseError>(act);
    }
}