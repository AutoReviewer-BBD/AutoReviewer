using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoReview.Models;

public partial class UserSkill
{
    [Key]
    [Column("userSkillID")]
    public int UserSkillId { get; set; }

    [Column("gitHubUserID")]
    public int GitHubUserId { get; set; }

    [Column("skillID")]
    public int SkillId { get; set; }

    [ForeignKey("GitHubUserId")]
    [InverseProperty("UserSkills")]
    public virtual GitHubUser GitHubUser { get; set; } = null!;

    [ForeignKey("SkillId")]
    [InverseProperty("UserSkills")]
    public virtual Skill Skill { get; set; } = null!;
}
