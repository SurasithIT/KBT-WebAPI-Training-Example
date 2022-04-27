using System;
using System.Collections.Generic;
using System.Linq;
using KBT.WebAPI.Training.Example.WebAPI.Repositories.Interfaces;
using KBT.WebAPI.Training.Example.WebAPI.Services;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;
using Moq;
using Xunit;

namespace KBT.WebAPI.Training.Example.Test.User;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public void Get_All_Users()
    {
        var expects = UserMockData.GetUsers();
        _userRepositoryMock.Setup(repository => repository.GetUsers()).Returns(expects);
        IUserService _userService = new UserService(_userRepositoryMock.Object);
        var result = _userService.GetUsers();

        Assert.IsType<List<Entities.Demo.User>>(result);
        Assert.Equal(expects, result);
    }
    
    [Fact]
    public void Get_UserBy_UserId()
    {
        var expect = UserMockData.GetUsers().First();
        _userRepositoryMock.Setup(repository => repository.GetUserById(1)).Returns(expect);
        IUserService _userService = new UserService(_userRepositoryMock.Object);
        var result = _userService.GetUserById(1);

        Assert.IsType<Entities.Demo.User>(result);
        Assert.Equal(expect, result);
    }
    
    [Fact]
    public void Insert_User()
    {
        var users = UserMockData.GetUsers();
        var expect = users.Count() + 1;
        _userRepositoryMock.Setup(repository => repository.Insert(It.IsAny<Entities.Demo.User>())).Callback((Entities.Demo.User user) => users.Add(user));
        IUserService _userService = new UserService(_userRepositoryMock.Object);
        var insertedId = _userService.Insert(UserMockData.NewUser());

        Assert.Equal(expect, users.Count());
    }
    
    [Fact]
    public void Update_User()
    {
        var user = UserMockData.GetUsers().First();

        var expect = UserMockData.GetUsers().First();
        expect.UserName = "testUpdateUserName";
        
        _userRepositoryMock.Setup(repository => repository.Update(user.UserKey, user)).Returns(expect);
        IUserService _userService = new UserService(_userRepositoryMock.Object);
        var result = _userService.Update(user.UserKey, user);

        Assert.Equal(expect, result);
    }
    
    [Fact]
    public void Delete_User()
    {
        var user = UserMockData.GetUsers().First();
        _userRepositoryMock.Setup(repository => repository.Delete(It.IsAny<int>()));
        IUserService _userService = new UserService(_userRepositoryMock.Object);
        _userService.Delete(user.UserKey);

        _userRepositoryMock.Verify(r => r.Delete(user.UserKey));
    }
}