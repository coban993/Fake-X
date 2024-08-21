using System.Data;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.DTOs;
using DotnetAPI2.Helpers;
using DotnetAPI2.Models;
using DotnetAPI2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSQL _reusableSQL;
    private readonly IUserService _userService;
    public UserCompleteController(IConfiguration config, IUserService userService)
    {
        _dapper = new DataContextDapper(config);
        _reusableSQL = new ReusableSQL(config);
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