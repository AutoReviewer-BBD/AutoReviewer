using Microsoft.EntityFrameworkCore;
using  RealConnection.Data;
using  RealConnection.Models;

// using AutoReviewerDbContext context = new AutoReviewerDbContext();

//Adding
// Skill newSkill = new Skill()
// {
//     SkillId = 1000,
//     SkillName = "New Skill"
// };
// context.Skills.Add(newSkill);
// context.SaveChanges();

//Select
// var skills = context.Skills.Where(skill => skill.SkillId == 1)
// var skills = from skill in context.Skills where skill.SkillId < 5 select skill;

// foreach (Skill skill in skills){
//     Console.WriteLine($"SkillID:\t{skill.SkillId}");
//     Console.WriteLine($"SkillName:\t{skill.SkillName}");
// }

// Updating
// var skill = context.Skills.Where(skill => skill.SkillId == 1).FirstOrDefault();
// if (skill != null)
// {
//     skill.SkillName = "New Skill Name";
// }
// context.SaveChanges();

//Deleting
// var skill = context.Skills.Where(skill => skill.SkillId == 1).FirstOrDefault();
// if (skill != null)
// {
//     context.Remove(skill);
// }
// context.SaveChanges();