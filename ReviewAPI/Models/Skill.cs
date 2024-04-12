using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RealConnection.Models;

[Table("Skill")]
[Index("SkillName", Name = "uq_skillName", IsUnique = true)]
public partial class Skill
{
    [Key]
    [Column("skillID")]
    public int SkillId { get; set; }

    [Column("skillName")]
    [StringLength(20)]
    [Unicode(false)]
    public string SkillName { get; set; } = null!;

    [InverseProperty("Skill")]
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
