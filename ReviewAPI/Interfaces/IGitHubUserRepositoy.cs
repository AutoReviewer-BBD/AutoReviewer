using RealConnection.Models;

namespace Api.Interfaces
{
    public interface IGitHubUserRepositoy
    {
        GitHubUser LoginUser(string userName);
        GitHubUser? GetUser(string userName);
        GitHubUser AddUser(GitHubUser gitHubUser);
    }
}