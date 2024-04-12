using Api.Interfaces;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Repositories
{
    public class UserSkillsRepository : IUserSkillsRepository
    {
        private readonly AutoReviewerDbContext dataContext;
        
        public UserSkillsRepository(AutoReviewerDbContext context){
            dataContext = context;
        }

        public ICollection<ProcedureUsersWithSkill> GetUsersWithSkillInRepository(int skillID, int repositoryID, int userID){
            var queryResult = dataContext.ViewUsersWithSkillInRepository(skillID, repositoryID, userID);
            return queryResult.ToList();
        }
        public UserSkill AddUserWithSkill(UserSkill userSkill){
            dataContext.UserSkills.Add(userSkill);
            dataContext.SaveChanges();
            return userSkill;
        }
    }
}