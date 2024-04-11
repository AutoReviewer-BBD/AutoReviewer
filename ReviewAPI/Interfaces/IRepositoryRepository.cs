using RealConnection.Models;

namespace Api.Interfaces
{
    public interface IRepositoryRepository
    {
        ICollection<ProcedureViewUsersRepositories> GetRepositoriesForUser(int gitHubUserID);
        ICollection<Repository> GetRepositories();
        Repository? GetRepositoryWithName(string repositoryName);
        Repository AddRepository(Repository repository);
    }
}