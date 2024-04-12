using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RealConnection.Models;

[Table("GitHubUser")]
[Index("GitHubUsername", Name = "uq_gitHubUsername", IsUnique = true)]
public partial class GitHubUser
{
    [Key]
    [Column("gitHubUserID")]
    public int GitHubUserId { get; set; }

    [Column("gitHubUsername")]
    [StringLength(39)]
    [Unicode(false)]
    public string GitHubUsername { get; set; } = null!;

    [InverseProperty("GitHubUser")]
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    [InverseProperty("GitHubUser")]
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
