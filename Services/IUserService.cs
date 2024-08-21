using DotnetAPI2.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Services
{
    public interface IUserService
    {
        IEnumerable<UserComplete> GetUsers(int userId, bool isActive);
        public bool UpsertUser(UserComplete user);
        bool DeleteUser(int userId);
    }
}