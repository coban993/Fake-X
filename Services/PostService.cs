using System.Data;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.Models;

namespace DotnetAPI2.Services
{
    public class PostService : IPostService
    {
        private readonly DataContextDapper _dapper;
        public PostService(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();
            if (postId != 0)
            {
                stringParameters += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam.ToLower() != "none")
            {
                stringParameters += ", @SearchValue=@SearchValueParameter";
                sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        public IEnumerable<Post> GetPostByUser(string userId)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            
            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        public bool UpsertPost(Post postToUpsert)
        {
            throw new NotImplementedException();
        }

        public bool DeletePost(int postId)
        {
            throw new NotImplementedException();
        }
    }
}