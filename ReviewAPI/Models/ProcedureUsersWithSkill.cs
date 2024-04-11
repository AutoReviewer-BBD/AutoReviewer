using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RealConnection.Models;

[Keyless]
public class ProcedureUsersWithSkill
{
    [Column("gitHubUsername")]
    public string GitHubUsername { get; set; } = null!;
    [Column("skillName")]
    public string SkillName { get; set; } = null!;
    [Column("repositoryName")]
    public string RepositoryName { get; set; } = null!;
}