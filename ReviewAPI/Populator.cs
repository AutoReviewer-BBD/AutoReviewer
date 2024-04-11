using Api.Repositories;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Populating;
public static class Populator {            
    public static void AddRegistrationsForUser(string username, List<Repository> repositories){
        AutoReviewerDbContext dataContext = new AutoReviewerDbContext();
        RepositoryRepository repositoryRepository = new RepositoryRepository(dataContext);
        GitHubUserRepositoy gitHubUserRepositoy = new GitHubUserRepositoy(dataContext);
        RegistrationRepository registrationRepository = new RegistrationRepository(dataContext);
        List<Repository> repositoriesWithIDs = new List<Repository>();

        int gitHubUserId = gitHubUserRepositoy.GetUser(username).GitHubUserId;

        // Adding repositiries if they dont exist.
        foreach (Repository repository in repositories){
            Repository? repositoryWithID = repositoryRepository.GetRepositoryWithName(repository.RepositoryName);
            bool alreadyExists = repositoryWithID != null;
            if (!alreadyExists){
                repositoryRepository.AddRepository(repository);
                repositoryWithID = repositoryRepository.GetRepositoryWithName(repository.RepositoryName);
            }
            repositoriesWithIDs.Add(repositoryWithID);
        }

        // Deleting old registrations
        registrationRepository.DeleteRelationships(gitHubUserId);

        // Adding new relationships
        foreach (Repository repository in repositoriesWithIDs){
            registrationRepository.SaveUserRegistration(new Registration(){
                GitHubUserId = gitHubUserId,
                RepositoryId = repository.RepositoryId
            });
        }
    }
}