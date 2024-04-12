using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Repositories
{
    public class RepositoryRepository : IRepositoryRepository
    {
        private readonly AutoReviewerDbContext dataContext;

        public RepositoryRepository(AutoReviewerDbContext context){
            dataContext = context;
        }

        public ICollection<Repository> GetRepositories(){
            DbSet<Repository> queryResult = dataContext.Repositories;
            return queryResult.ToList();
        }

        public Repository? GetRepositoryWithName(string repositoryName){
            Repository? queryResult = dataContext.Repositories
                                .Where(repository => repository.RepositoryName == repositoryName)
                                .FirstOrDefault();
            return queryResult;
        }

        public Repository AddRepository(Repository repository){
            dataContext.Repositories.Add(repository);
            dataContext.SaveChanges();
            return repository;
        }

        public ICollection<ProcedureViewUsersRepositories> GetRepositoriesForUser(string gitHubUsername){
            List<ProcedureViewUsersRepositories> queryResult = dataContext.GetRepositoriesForUser(gitHubUsername);
            return queryResult.ToList();
        }
    }
}