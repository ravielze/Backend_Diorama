using Diorama.RestAPI.Services;
using Xunit;
using Moq;
using Diorama.Internals.Persistent.Models;
using Diorama.Internals.Contract;
using System;
using System.Net;
using Diorama.Internals.Responses;
using System.Text.Json;

namespace DioramaTest;

public class UserServiceTests : Tester
{

    public UserServiceTests()
    {
    }

    private IUserService Setup(Action<MockWrappers> config)
    {
        MockWrappers _mw = new MockWrappers();
        config.Invoke(_mw);
        IUserService _service = new UserService(_mw.UserRepo.Object, _mw.FollowerRepo.Object, _mw.Hasher.Object);
        return _service;
    }

    [Fact]
    public void Authenticate_When_User_Not_Found()
    {
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.Authenticate(
                new AuthContract()
            );
        });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Username not found.", response.Details);
    }

    [Fact]
    public void Authenticate_When_Password_Invalid()
    {
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockUser);
            mw.Hasher.Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        });

        ResponseError response = IsError(() =>
        {
            _service.Authenticate(
                new AuthContract()
            );
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Invalid password.", response.Details);
    }

    [Fact]
    public void Authenticate_When_Successful()
    {
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockUser);
            mw.Hasher.Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        });

        ResponseOK response = IsOK(() =>
        {
            _service.Authenticate(
                new AuthContract()
            );
        });
        UserAuthContract details = Assert.IsAssignableFrom<UserAuthContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new UserContract(mockUser));
        string actualResponse = JsonSerializer.Serialize(details.User);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void Register_When_User_Found()
    {
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockUser);
        });

        ResponseError response = IsError(() =>
        {
            _service.Register(
                new RegisterAuthContract()
            );
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Username is already registered.", response.Details);
    }

    [Fact]
    public void Register_When_Successful()
    {
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
            mw.UserRepo.Setup(p => p.CreateNormalUser(It.IsAny<User>())).Returns(mockUser);
        });

        ResponseOK response = IsOK(() =>
        {
            _service.Register(
                new RegisterAuthContract()
            );
        });
        UserContract details = Assert.IsAssignableFrom<UserContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new UserContract(mockUser));
        string actualResponse = JsonSerializer.Serialize(details);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void EditUserProfile_When_User_Not_Found()
    {
        int mockUserId = 1;
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.EditUserProfile(
                mockUserId,
                new EditUserContract()
            );
        });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void EditUserProfile_When_Picking_Invalid_Username()
    {
        int mockUserId = 1;
        User mockUser = new User();
        User otherUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(mockUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(otherUser);
        });

        ResponseError response = IsError(() =>
        {
            _service.EditUserProfile(
                mockUserId,
                new EditUserContract()
                {
                    Username = "biar_beda"
                }
            );
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("User with spesific username already exist.", response.Details);
    }

    [Fact]
    public void EditUserProfile_When_Successful()
    {
        int mockUserId = 1;
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(mockUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
        });

        ResponseOK response = IsOK(() =>
        {
            _service.EditUserProfile(
                mockUserId,
                new EditUserContract()
            );
        });
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Success to edit profile.", response.Details);
    }

    [Fact]
    public void GetUserProfile_When_User_Not_Found()
    {
        int mockUserId = 1;
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.GetUserProfile(
                mockUserId
            );
        });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void GetUserProfile_When_Successful()
    {
        int mockUserId = 1;
        User mockUser = new User();
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(mockUser);
        });

        ResponseOK response = IsOK(() =>
        {
            _service.GetUserProfile(
                mockUserId
            );
        });
        UserContract details = Assert.IsAssignableFrom<UserContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new UserContract(mockUser));
        string actualResponse = JsonSerializer.Serialize(details);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void Follow_When_Subject_User_Not_Found()
    {
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.Follow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void Follow_When_Target_User_Not_Found()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.Follow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Username not found.", response.Details);
    }

    [Fact]
    public void Follow_When_Follow_Twice()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;

        User mockTargetUser = new User();
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockTargetUser);
            mw.FollowerRepo
                .Setup(p => p.CreateFollower(It.IsAny<Follower>()))
                .Callback((Follower mockFollowerInstance) => {
                    throw new ArgumentException("Failed create instance follower");
                });
        });

        ResponseError response = IsError(() =>
        {
            _service.Follow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Can't follow twice", response.Details);
    }

    [Fact]
    public void Follow_When_Successful()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;

        User mockTargetUser = new User();
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockTargetUser);
            mw.FollowerRepo
                .Setup(p => p.CreateFollower(It.IsAny<Follower>()))
                .Verifiable();
            mw.UserRepo
                .Setup(p => 
                    p.UpdateFollowersFollowingTotal(
                        It.IsAny<User>(), 
                        It.IsAny<User>(), 
                        "follow"
                    )
                )
                .Verifiable();
        });

        ResponseOK response = IsOK(() =>
        {
            _service.Follow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Follow success", response.Details);
    }

    [Fact]
    public void Unfollow_When_Subject_User_Not_Found()
    {
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.Unfollow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void Unfollow_When_Target_User_Not_Found()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.Unfollow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Username not found.", response.Details);
    }

    [Fact]
    public void Unfollow_When_Follow_Twice()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;

        User mockTargetUser = new User();
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockTargetUser);
            mw.FollowerRepo
                .Setup(p => p.DeleteFollower(It.IsAny<Follower>()))
                .Callback((Follower mockFollowerInstance) => {
                    throw new ArgumentException("Failed remove instance follower");
                });
        });

        ResponseError response = IsError(() =>
        {
            _service.Unfollow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Can't unfollow twice", response.Details);
    }

    [Fact]
    public void Unfollow_When_Successful()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;

        User mockTargetUser = new User();
        string mockUserTargetUsername = "dummy";
        
        // Follower mockFollowerInstance = new Follower(mockSubjectUser, mockTargetUser);
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockTargetUser);
            mw.FollowerRepo
                .Setup(p => p.DeleteFollower(It.IsAny<Follower>()))
                .Verifiable();
            mw.UserRepo
                .Setup(p => 
                    p.UpdateFollowersFollowingTotal(
                        It.IsAny<User>(), 
                        It.IsAny<User>(), 
                        It.IsAny<string>()
                    )
                )
                .Verifiable();
        });

        ResponseOK response = IsOK(() =>
        {
            _service.Unfollow(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Unfollow success.", response.Details);
    }

    [Fact]
    public void IsFollowing_When_Subject_User_Not_Found()
    {
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.isFollowing(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void IsFollowing_When_Target_User_Not_Found()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.isFollowing(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Username not found.", response.Details);
    }

    [Fact]
    public void IsFollowing_When_Successful()
    {
        User mockSubjectUser = new User();
        int mockUserSubjectId = 1;

        User mockTargetUser = new User();
        string mockUserTargetUsername = "dummy";
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockSubjectUser);
            mw.UserRepo.Setup(p => p.Find(It.IsAny<string>())).Returns(mockTargetUser);
            mw.FollowerRepo
                .Setup(p => p.Find(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<User?>(null);
        });

        ResponseOK response = IsOK(() =>
        {
            _service.isFollowing(mockUserSubjectId, mockUserTargetUsername);
        });

        Assert.IsAssignableFrom<bool>(response.Details);
        Assert.Equal(false, response.Details);
    }
}