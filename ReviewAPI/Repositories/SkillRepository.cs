using Api.Interfaces;
using RealConnection.Data;
using RealConnection.Models;

namespace Api.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AutoReviewerDbContext dataContext;
        
        public SkillRepository(AutoReviewerDbContext context){
            dataContext = context;
        }

        public ICollection<Skill> GetAllSkills(){
            return dataContext.Skills.ToList();
        }
    }
}