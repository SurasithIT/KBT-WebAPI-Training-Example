using System;
using System.Collections.Generic;

namespace KBT.WebAPI.Training.Example.Test.User;

public class UserMockData
{
    public static List<Entities.Demo.User> GetUsers()
    {
        List<Entities.Demo.User> users = new List<Entities.Demo.User>()
        {
            new Entities.Demo.User()
            {
                UserKey = 1, UserName = "TestUser", Password = "badpassword", IsActive = true, EmployeeKey = null
            },
            new Entities.Demo.User()
            {
                UserKey = 2, UserName = "TestUser2", Password = "badpassword", IsActive = true, EmployeeKey = null
            },
            new Entities.Demo.User()
            {
                UserKey = 3, UserName = "TestUser3", Password = "badpassword", IsActive = true, EmployeeKey = null
            },
        };
        return users;
    }

    public static Entities.Demo.User NewUser()
    {
        return new Entities.Demo.User()
        {
            UserKey = 4, UserName = "TestUser4", Password = "badpassword", IsActive = true, EmployeeKey = null
        };
    }
}