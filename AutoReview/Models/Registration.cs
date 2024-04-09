using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoReview.Models;

[Table("Registration")]
public partial class Registration
{
    [Key]
    [Column("registrationID")]
    public int RegistrationId { get; set; }

    [Column("gitHubUserID")]
    public int GitHubUserId { get; set; }

    [Column("repositoryID")]
    public int RepositoryId { get; set; }

    [ForeignKey("GitHubUserId")]
    [InverseProperty("Registrations")]
    public virtual GitHubUser GitHubUser { get; set; } = null!;

    [ForeignKey("RepositoryId")]
    [InverseProperty("Registrations")]
    public virtual Repository Repository { get; set; } = null!;
}
