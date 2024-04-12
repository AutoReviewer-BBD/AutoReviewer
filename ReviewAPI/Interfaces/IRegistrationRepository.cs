using RealConnection.Models;

namespace Api.Interfaces
{
    public interface IRegistrationRepository
    {
        ICollection<Registration> GetUserRegistrations(int userId);
        Registration SaveUserRegistration(Registration registration);
        void DeleteRelationships(int userID);
    }
}