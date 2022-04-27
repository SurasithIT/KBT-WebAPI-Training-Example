using KBT.WebAPI.Training.Example.Entities.Demo;

namespace KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;

public interface IUserService
{
    public IEnumerable<User> GetUsers();
    public User? GetUserById(int userKey);
    public int? Insert(User jwt);
    public User Update(int userKey, User user);
    public void Delete(int userKey);
}