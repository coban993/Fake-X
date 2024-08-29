using System.Data;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.Models;
using DotnetAPI2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IPostService _postService;
        public PostController(IConfiguration config, IPostService postService)
        {
            _dapper = new DataContextDapper(config);
            _postService = postService;
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IActionResult GetPosts(int postId, int userId, string searchParam)
        {
            return Ok(_postService.GetPosts(postId, userId, searchParam));
        }

        [HttpGet("MyPosts")]
        public IActionResult GetPostByUser()
        {
            string? userId = User.FindFirst("userId")?.Value;
            
            return Ok(_postService.GetPostByUser(userId));
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId=@UserIdParameter, 
                @PostTitle=@PostTitleParameter, 
                @PostContent=@PostContentParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Unable to create new post");
        }

        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete 
                @UserId=@UserIdParameter, 
                @PostId=@PostIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}