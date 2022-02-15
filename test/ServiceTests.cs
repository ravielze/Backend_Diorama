using Diorama.Internals.Resource;
using Diorama.Internals.Responses;
using Diorama.RestAPI.Repositories;
using Moq;
using System;
using Xunit;

namespace DioramaTest;

public class MockWrappers {

    public Mock<IUserRepository> UserRepo = new Mock<IUserRepository>();
    public Mock<IHasher> Hasher = new Mock<IHasher>();
}

public abstract class Tester {

    protected void IsOK(Action act) {
        Assert.Throws<ResponseOK>(act);
    }

    protected void IsError(Action act) {
        Assert.Throws<ResponseError>(act);
    }
}