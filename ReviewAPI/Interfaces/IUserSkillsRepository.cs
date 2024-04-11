using RealConnection.Models;

namespace Api.Interfaces
{
    public interface IUserSkillsRepository
    {
        UserSkill AddUserWithSkill(UserSkill userSkill);
        ICollection<ProcedureUsersWithSkill> GetUsersWithSkillInRepository(int skillID, int repositoryID, int userID);
    }
}