using Microsoft.EntityFrameworkCore;
using HRCandidateManagement.Models;

namespace HRCandidateManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Skill> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Candidate>()
            .HasMany(c => c.Skills)
            .WithMany(s => s.Candidates)
            .UsingEntity(j => j.ToTable("CandidateSkill"));

        modelBuilder.Entity<Candidate>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique();

        var skills = new[]
        {
            new Skill { Id = 1, Name = "Java" },
            new Skill { Id = 2, Name = "C#" },
            new Skill { Id = 3, Name = "Database Design" },
            new Skill { Id = 4, Name = "English" },
            new Skill { Id = 5, Name = "Russian" },
            new Skill { Id = 6, Name = "German" },
            new Skill { Id = 7, Name = "Python" },
            new Skill { Id = 8, Name = "SQL" }
        };

        modelBuilder.Entity<Skill>().HasData(skills);

        var candidates = new[]
        {
            new Candidate
            {
                Id = 1,
                FullName = "Marko Markovic",
                DateOfBirth = new DateOnly(2002, 3, 14),
                ContactNumber = "066-1234-567",
                Email = "markomarkovic@gmail.com"
            },
            new Candidate
            {
                Id = 2,
                FullName = "Žarko Žarkovic",
                DateOfBirth = new DateOnly(2003, 7, 22),
                ContactNumber = "066-2345-678",
                Email = "zarkozarkovic@gmail.com"
            },
            new Candidate
            {
                Id = 3,
                FullName = "Nikola Nikolic",
                DateOfBirth = new DateOnly(2002, 11, 5),
                ContactNumber = "066-3456-789",
                Email = "nikolanikolic@gmail.com"
            },
            new Candidate
            {
                Id = 4,
                FullName = "Jovana Jovanovic",
                DateOfBirth = new DateOnly(2003, 1, 18),
                ContactNumber = "066-4567-890",
                Email = "jovanajovanovic@gmail.com"
            },
            new Candidate
            {
                Id = 5,
                FullName = "Stefan Stefanovic",
                DateOfBirth = new DateOnly(2002, 9, 30),
                ContactNumber = "066-5678-901",
                Email = "stefanstefanovic@gmail.com"
            }
        };

        modelBuilder.Entity<Candidate>().HasData(candidates);

        modelBuilder.Entity<Candidate>()
            .HasMany(c => c.Skills)
            .WithMany(s => s.Candidates)
            .UsingEntity(j =>
                j.HasData(
                    new { CandidatesId = 1, SkillsId = 1 },
                    new { CandidatesId = 1, SkillsId = 2 },
                    new { CandidatesId = 1, SkillsId = 3 },
                    new { CandidatesId = 1, SkillsId = 4 },
                    new { CandidatesId = 2, SkillsId = 2 },
                    new { CandidatesId = 2, SkillsId = 3 },
                    new { CandidatesId = 2, SkillsId = 5 },
                    new { CandidatesId = 3, SkillsId = 1 },
                    new { CandidatesId = 3, SkillsId = 4 },
                    new { CandidatesId = 3, SkillsId = 6 },
                    new { CandidatesId = 4, SkillsId = 7 },
                    new { CandidatesId = 4, SkillsId = 8 },
                    new { CandidatesId = 5, SkillsId = 2 },
                    new { CandidatesId = 5, SkillsId = 7 }
                )
            );
    }
}
