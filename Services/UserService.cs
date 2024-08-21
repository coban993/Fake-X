using System.Data;
using AutoMapper;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Services
{
    public class UserService : IUserService
    {
        private readonly DataContextDapper _dapper;

        public UserService(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

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

        public bool DeleteUser(int userId)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Delete
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return true;
            }

            throw new Exception("Failed to delete user");
        }
    }
}