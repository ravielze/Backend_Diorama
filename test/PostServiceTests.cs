using Diorama.RestAPI.Services;
using Xunit;
using Moq;
using Diorama.Internals.Persistent.Models;
using Diorama.Internals.Contract;
using System;
using System.Collections;
using System.Net;
using Diorama.Internals.Responses;
using System.Text.Json;
using System.Collections.Generic;

namespace DioramaTest;

public class PostServiceTests : Tester
{
    public PostServiceTests()
    {
    }

    private IPostService Setup(Action<MockWrappers> config)
    {
        MockWrappers _mw = new MockWrappers();

        config.Invoke(_mw);

        IPostService _service = new PostService(_mw.PostRepo.Object, _mw.UserRepo.Object);

        return _service;
    }

    [Fact]
    public void Create_Post_When_User_Not_Found() {
        int mockUserId = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.CreatePost(mockUserId, new CreatePostContract());
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void Create_Post_When_Successful()
    {
        Post mockPost = new Post(new User(), new CreatePostContract());

        int mockUserId = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.Create(mockPost)).Verifiable();
        });

        ResponseOK response = IsOK(() =>
        {
            _service.CreatePost(mockUserId,new CreatePostContract());
        });

        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Post created.", response.Details);
    }

    [Fact]
    public void Get_Post_For_Home_Page_When_User_Not_Found()
    {
        int mockUserId = 1;
        int mockPage = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.GetPostForHomePage(mockUserId, mockPage);
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }

    [Fact]
    public void Get_Post_For_Home_Page_When_Success()
    {
        IEnumerable<Post> posts = new List<Post>();

        int mockUserId = 1;
        int mockPage = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.GetNewest(mockUserId, mockPage)).Returns((posts, 1, 0));
        });


        ResponseOK response = IsOK(() =>
        {
            _service.GetPostForHomePage(mockUserId, mockPage);
        });

        PostsContract details = Assert.IsAssignableFrom<PostsContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new PostsContract(posts, 1, 1));
        string actualResponse = JsonSerializer.Serialize(details);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void Get_Post_For_Explore_Page_When_User_Not_Found()
    {
        int mockUserId = 1;
        int mockPage = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns<User?>(null);
        });

        ResponseError response = IsError(() =>
        {
            _service.GetPostForExplorePage(mockUserId, mockPage);
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Data inconsistent.", response.Details);
    }


    [Fact]
    public void Get_Post_For_Explore_Page_When_Page_Greater_Than_Max_Page()
    {
        int mockUserId = 1;
        int mockPage = 1;

        IEnumerable<Post> posts = new List<Post>();

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.GetNewestExplore(mockPage)).Returns((posts, 10, 2));
        });


        ResponseError response = IsError(() =>
        {
            _service.GetPostForExplorePage(mockUserId, mockPage);
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Page not found.", response.Details);
    }

    [Fact]
    public void Get_Post_For_Explore_Page_When_Success()
    {
        int mockUserId = 1;
        int mockPage = 1;

        IEnumerable<Post> posts = new List<Post>();

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.GetNewestExplore(mockPage)).Returns((posts, 1, 0));
        });


        ResponseOK response = IsOK(() =>
        {
            _service.GetPostForExplorePage(mockUserId, mockPage);
        });

        PostsContract details = Assert.IsAssignableFrom<PostsContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new PostsContract(posts, 1, 1));
        string actualResponse = JsonSerializer.Serialize(details);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void Get_Spesific_Post_When_Not_Found()
    {
        int mockUserId = 1;
        int mockPageId = 1;

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns<Post?>(null);
        });


        ResponseError response = IsError(() =>
        {
            _service.GetSpesificPost(mockUserId, mockPageId);
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Post with spesific id not found.", response.Details);
    }

    [Fact]
    public void Get_Spesific_Post_When_Success()
    {
        int mockUserId = 1;
        int mockPageId = 1;

        Post mockPost = new Post();

        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(mockUserId)).Returns(new User());
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns(mockPost);
        });


        ResponseOK response = IsOK(() =>
        {
            _service.GetSpesificPost(mockUserId, mockPageId);
        });

        PostContract details = Assert.IsAssignableFrom<PostContract>(response.Details);
        string expectedResponse = JsonSerializer.Serialize(new PostContract(mockPost));
        string actualResponse = JsonSerializer.Serialize(details);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void Like_When_Like_Twice()
    {
        User mockUser = new User();
        Post mockPost = new Post();

        int mockUserId = 1;
        int mockPageId = 1;
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockUser);
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns(mockPost);
            mw.PostRepo
                .Setup(p => p.Create(It.IsAny<PostLike>()))
                .Callback((PostLike mockPostLikeInstance) => {
                    throw new ArgumentException("Can't like twice");
                });
        });

        ResponseError response = IsError(() =>
        {
            _service.LikePost(mockUserId, mockPageId);
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Can't like twice", response.Details);
    }

    [Fact]
    public void Like_When_Like_Success()
    {
        User mockUser = new User();
        Post mockPost = new Post();

        int mockUserId = 1;
        int mockPageId = 1;
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockUser);
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns(mockPost);
            mw.PostRepo
                .Setup(p => p.Create(It.IsAny<PostLike>()))
                .Returns(new PostLike(mockUser, mockPost));
        });

        ResponseOK response = IsOK(() =>
        {
            _service.LikePost(mockUserId, mockPageId);
        });

        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Like Success", response.Details);
    }

    [Fact]
    public void Unlike_When_Unlike_Twice()
    {
        User mockUser = new User();
        Post mockPost = new Post();

        int mockUserId = 1;
        int mockPageId = 1;
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockUser);
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns(mockPost);
            mw.PostRepo
                .Setup(p => p.Delete(It.IsAny<PostLike>()))
                .Callback((PostLike mockPostLikeInstance) => {
                    throw new ArgumentException("Can't unlike twice");
                });
        });

        ResponseError response = IsError(() =>
        {
            _service.UnlikePost(mockUserId, mockPageId);
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Can't unlike twice", response.Details);
    }

    [Fact]
    public void Unlike_When_Unlike_Success()
    {
        User mockUser = new User();
        Post mockPost = new Post();

        int mockUserId = 1;
        int mockPageId = 1;
        
        var _service = Setup((mw) =>
        {
            mw.UserRepo.Setup(p => p.FindById(It.IsAny<int>())).Returns(mockUser);
            mw.PostRepo.Setup(p => p.FindById(mockPageId)).Returns(mockPost);
            mw.PostRepo
                .Setup(p => p.Delete(It.IsAny<PostLike>()))
                .Verifiable();
        });

        ResponseOK response = IsOK(() =>
        {
            _service.UnlikePost(mockUserId, mockPageId);
        });

        Assert.IsAssignableFrom<string>(response.Details);
        Assert.Equal("Unlike Success", response.Details);
    }
}