using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RealConnection.Models;
[Keyless]
public class ProcedureViewUsersRepositories
{
    [Column("repositoryID")]
    public int RepositoryId { get; set; }

    [Column("repositoryName")]
    [Unicode(false)]
    public string RepositoryName { get; set; } = null!;

    [Column("repositoryOwnerUsername")]
    [Unicode(false)]
    public string RepositoryOwnerUsername { get; set; } = null!;
}