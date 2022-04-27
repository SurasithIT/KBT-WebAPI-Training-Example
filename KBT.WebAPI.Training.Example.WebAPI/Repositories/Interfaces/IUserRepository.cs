using KBT.WebAPI.Training.Example.Entities.Demo;

namespace KBT.WebAPI.Training.Example.WebAPI.Repositories.Interfaces;

public interface IUserRepository
{
    public IEnumerable<User> GetUsers();
    public User? GetUserById(int userKey);
    public int? Insert(User user);
    public User Update(int userKey, User user);
    public void Delete(int userKey);
}