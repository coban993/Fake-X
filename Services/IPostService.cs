using DotnetAPI2.Models;

namespace DotnetAPI2.Services
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None");
        IEnumerable<Post> GetPostByUser(string userId);
        bool UpsertPost(Post postToUpsert);
        bool DeletePost(int postId);
    }
}