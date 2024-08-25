using DotnetAPI2.Models;

namespace DotnetAPI2.Services
{
    public interface IUserService
    {
        IEnumerable<UserComplete> GetUsers(int userId, bool isActive);
        bool UpsertUser(UserComplete user);
        bool DeleteUser(int userId);
    }
}