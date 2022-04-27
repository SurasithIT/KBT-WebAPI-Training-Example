using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.WebAPI.Repositories.Interfaces;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;

namespace KBT.WebAPI.Training.Example.WebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    public User? GetUserById(int userKey)
    {
        return _userRepository.GetUserById(userKey);
    }

    public int? Insert(User user)
    {
        return _userRepository.Insert(user);
    }

    public User Update(int userKey, User user)
    {
        return _userRepository.Update(userKey, user);
    }

    public void Delete(int userKey)
    {
        _userRepository.Delete(userKey);
    }
}