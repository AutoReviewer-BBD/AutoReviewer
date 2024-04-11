using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RealConnection.Models;

namespace RealConnection.Data;

public partial class AutoReviewerDbContext : DbContext
{
    public AutoReviewerDbContext()
    {
    }

    public AutoReviewerDbContext(DbContextOptions<AutoReviewerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FlywaySchemaHistory> FlywaySchemaHistories { get; set; }

    public virtual DbSet<GitHubUser> GitHubUsers { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Repository> Repositories { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<ProcedureUsersWithSkill> ProcedureUsersWithSkill {get; set;}
    public virtual DbSet<ProcedureViewUsersRepositories> ProcedureViewUsersRepositories {get; set;}

    public virtual DbSet<UserSkill> UserSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=autoreviewerdb.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com,1433;Initial Catalog=AutoReviewerDB;User ID=AutoreviewerAdmin;Password=Password12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlywaySchemaHistory>(entity =>
        {
            entity.HasKey(e => e.InstalledRank).HasName("flyway_schema_history_pk");

            entity.Property(e => e.InstalledRank).ValueGeneratedNever();
            entity.Property(e => e.InstalledOn).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<GitHubUser>(entity =>
        {
            entity.HasKey(e => e.GitHubUserId).HasName("pk_gitHubUserID");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("pk_registrationID");

            entity.HasOne(d => d.GitHubUser).WithMany(p => p.Registrations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gitHubUserID_Registration");

            entity.HasOne(d => d.Repository).WithMany(p => p.Registrations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_repositoryID_Registration");
        });

        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => e.RepositoryId).HasName("pk_repositoryID");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("pk_skillID");
        });

        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.HasKey(e => e.UserSkillId).HasName("pk_userSkillID");

            entity.HasOne(d => d.GitHubUser).WithMany(p => p.UserSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gitHubUserID_UserSkills");

            entity.HasOne(d => d.Skill).WithMany(p => p.UserSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_skillID_UserSkills");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public List<ProcedureUsersWithSkill> ViewUsersWithSkillInRepository(int skillID, int repositoryID, int userID)
    {
        return ProcedureUsersWithSkill.FromSqlRaw("EXEC ViewUsersWithSkillInRepository @userID, @skillID, @repositoryID", 
            new SqlParameter("@userID", userID),
            new SqlParameter("@skillID", skillID),
            new SqlParameter("@repositoryID", repositoryID))
            .ToList();
    }

    public List<ProcedureViewUsersRepositories> GetRepositoriesForUser(string gitHubUsername)
    {
        return ProcedureViewUsersRepositories.FromSqlRaw("EXEC ViewUserRepositories @gitHubUsername", 
            new SqlParameter("@gitHubUsername", gitHubUsername))
            .ToList();
    }
}