using Api.Interfaces;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly AutoReviewerDbContext dataContext;
        
        public RegistrationRepository(AutoReviewerDbContext context){
            dataContext = context;
        }

        public ICollection<Registration> GetUserRegistrations(int userId){
            var queryResult = dataContext.Registrations.Where(
                registrations => registrations.GitHubUserId == userId
            );
            
            return queryResult.ToList();
        }
        public Registration SaveUserRegistration(Registration registration){
            dataContext.Registrations.Add(registration);
            dataContext.SaveChanges();
            return registration;
        }

        public void DeleteRelationships(int userID){
            List<Registration> registrationsToDelete = 
                dataContext.Registrations
                .Where(registration => registration.GitHubUserId == userID)
                .ToList();
            
            if (registrationsToDelete.Any()){
                dataContext.RemoveRange(registrationsToDelete);
                dataContext.SaveChanges();
            }
        }
    }
}