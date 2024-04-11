using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RealConnection.Models;

[Table("Repository")]
[Index("RepositoryName", Name = "uq_repositoryName", IsUnique = true)]
public partial class Repository
{
    [Key]
    [Column("repositoryID")]
    public int RepositoryId { get; set; }

    [Column("repositoryName")]
    [StringLength(100)]
    [Unicode(false)]
    public string RepositoryName { get; set; } = null!;

    [Column("repositoryOwnerUsername")]
    [StringLength(39)]
    [Unicode(false)]
    public string RepositoryOwnerUsername { get; set; } = null!;

    [InverseProperty("Repository")]
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
