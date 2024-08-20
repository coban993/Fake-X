using System.Data;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.DTOs;
using DotnetAPI2.Helpers;
using DotnetAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSQL _reusableSQL;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSQL = new ReusableSQL(config);
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }

        return _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        if (_reusableSQL.UpsertUser(user))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
            @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user");
    }
}