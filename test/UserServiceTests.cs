using Diorama.RestAPI.Services;
using Xunit;
using Moq;
using Diorama.Internals.Persistent.Models;
using Diorama.Internals.Contract;
using System;

namespace DioramaTest;

public class UserServiceTests : Tester {

    public UserServiceTests(){
    }
    
    private IUserService Setup(Action<MockWrappers> config) {
        MockWrappers _mw = new MockWrappers();
        config.Invoke(_mw);
        IUserService _service = new UserService(_mw.UserRepo.Object, _mw.Hasher.Object);
        return _service;
    }
    
    [Fact]
    public void Test1(){
        var _service = Setup((mw) => {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
            mw.UserRepo.Setup(p => p.CreateNormalUser(It.IsAny<User>())).Returns(new User());
        });

        IsOK(() => {
            _service.Register(
                new RegisterAuthContract(){
                    Name = "asdfghjkl"
                }
            );
        });
    }

    [Fact]
    public void Test21(){
        var _service = Setup((mw) => {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(new User());
            mw.UserRepo.Setup(p => p.CreateNormalUser(It.IsAny<User>())).Returns(new User());
        });

        IsError(() => {
            _service.Register(
                new RegisterAuthContract(){
                    Name = "asdfghjkl"
                }
            );
        });
    }
}