using RealConnection.Models;

namespace Api.Interfaces
{
    public interface ISkillRepository
    {
        ICollection<Skill> GetAllSkills();
    }
}