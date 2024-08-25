using DotnetAPI2.Models;
using DotnetAPI2.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly IUserService _userService;
    public UserCompleteController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IActionResult GetUsers(int userId, bool isActive)
    {
        return Ok(_userService.GetUsers(userId, isActive));
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        return Ok(_userService.UpsertUser(user));
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        return Ok(_userService.DeleteUser(userId));
    }
}