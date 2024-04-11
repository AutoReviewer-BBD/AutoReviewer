using Api.Repositories;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Populator;
public static class Populator {            
    public static void AddRegistrationsForUser(string username, List<Repository> repositories){
        AutoReviewerDbContext dataContext = new AutoReviewerDbContext();
        RepositoryRepository repositoryRepository = new RepositoryRepository(dataContext);
        GitHubUserRepositoy gitHubUserRepositoy = new GitHubUserRepositoy(dataContext);
        RegistrationRepository registrationRepository = new RegistrationRepository(dataContext);

        int gitHubUserId = gitHubUserRepositoy.GetUser(username).GitHubUserId;

        // Adding repositiries if they dont exist.
        foreach (Repository repository in repositories){
            bool alreadyExists = repositoryRepository.GetRepositoryWithName(repository.RepositoryName) != null;
            if (!alreadyExists){
                repositoryRepository.AddRepository(repository);
            }
        }

        // Deleting old registrations
        registrationRepository.DeleteRelationships(gitHubUserId);

        // Adding new relationships
        foreach (Repository repository in repositories){
            registrationRepository.SaveUserRegistration(new Registration(){
                GitHubUserId = gitHubUserId,
                RepositoryId = repository.RepositoryId
            });
        }
    }
}