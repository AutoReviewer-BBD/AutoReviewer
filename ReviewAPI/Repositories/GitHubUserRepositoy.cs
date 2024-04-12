using Api.Interfaces;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Repositories
{
    public class GitHubUserRepositoy : IGitHubUserRepositoy
    {
        private readonly AutoReviewerDbContext dataContext;

        public GitHubUserRepositoy(AutoReviewerDbContext context){
            dataContext = context;
        }

        public GitHubUser? GetUser(string userName){
            GitHubUser? queryResult = dataContext.GitHubUsers.Where(
                user => user.GitHubUsername == userName
            ).FirstOrDefault();
            
            return queryResult;
        }
        public GitHubUser AddUser(GitHubUser gitHubUser){
            dataContext.GitHubUsers.Add(gitHubUser);
            dataContext.SaveChanges();
            return gitHubUser;
        }

        public GitHubUser LoginUser(string userName){
            GitHubUser? gitHubUser = GetUser(userName);

            if (gitHubUser == null){
                gitHubUser = AddUser(new GitHubUser(){
                    GitHubUsername = userName
                });
            }

            return gitHubUser;
        }
    }
}