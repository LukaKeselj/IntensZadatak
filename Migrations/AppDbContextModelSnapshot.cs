using HRCandidateManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace HRCandidateManagement.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("CandidateSkill", b =>
                {
                    b.Property<int>("CandidatesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SkillsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CandidatesId", "SkillsId");

                    b.HasIndex("SkillsId");

                    b.ToTable("CandidateSkill", (string)null);

                    b.HasData(
                        new
                        {
                            CandidatesId = 1,
                            SkillsId = 1
                        },
                        new
                        {
                            CandidatesId = 1,
                            SkillsId = 2
                        },
                        new
                        {
                            CandidatesId = 1,
                            SkillsId = 3
                        },
                        new
                        {
                            CandidatesId = 1,
                            SkillsId = 4
                        },
                        new
                        {
                            CandidatesId = 2,
                            SkillsId = 2
                        },
                        new
                        {
                            CandidatesId = 2,
                            SkillsId = 3
                        },
                        new
                        {
                            CandidatesId = 2,
                            SkillsId = 5
                        },
                        new
                        {
                            CandidatesId = 3,
                            SkillsId = 1
                        },
                        new
                        {
                            CandidatesId = 3,
                            SkillsId = 4
                        },
                        new
                        {
                            CandidatesId = 3,
                            SkillsId = 6
                        },
                        new
                        {
                            CandidatesId = 4,
                            SkillsId = 7
                        },
                        new
                        {
                            CandidatesId = 4,
                            SkillsId = 8
                        },
                        new
                        {
                            CandidatesId = 5,
                            SkillsId = 2
                        },
                        new
                        {
                            CandidatesId = 5,
                            SkillsId = 7
                        });
                });

            modelBuilder.Entity("HRCandidateManagement.Models.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Candidates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ContactNumber = "066-1234-567",
                            DateOfBirth = new DateOnly(2002, 3, 14),
                            Email = "markomarkovic@gmail.com",
                            FullName = "Marko Markovic"
                        },
                        new
                        {
                            Id = 2,
                            ContactNumber = "066-2345-678",
                            DateOfBirth = new DateOnly(2003, 7, 22),
                            Email = "zarkozarkovic@gmail.com",
                            FullName = "Žarko Žarkovic"
                        },
                        new
                        {
                            Id = 3,
                            ContactNumber = "066-3456-789",
                            DateOfBirth = new DateOnly(2002, 11, 5),
                            Email = "nikolanikolic@gmail.com",
                            FullName = "Nikola Nikolic"
                        },
                        new
                        {
                            Id = 4,
                            ContactNumber = "066-4567-890",
                            DateOfBirth = new DateOnly(2003, 1, 18),
                            Email = "jovanajovanovic@gmail.com",
                            FullName = "Jovana Jovanovic"
                        },
                        new
                        {
                            Id = 5,
                            ContactNumber = "066-5678-901",
                            DateOfBirth = new DateOnly(2002, 9, 30),
                            Email = "stefanstefanovic@gmail.com",
                            FullName = "Stefan Stefanovic"
                        });
                });

            modelBuilder.Entity("HRCandidateManagement.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Skills");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Java"
                        },
                        new
                        {
                            Id = 2,
                            Name = "C#"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Database Design"
                        },
                        new
                        {
                            Id = 4,
                            Name = "English"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Russian"
                        },
                        new
                        {
                            Id = 6,
                            Name = "German"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Python"
                        },
                        new
                        {
                            Id = 8,
                            Name = "SQL"
                        });
                });

            modelBuilder.Entity("CandidateSkill", b =>
                {
                    b.HasOne("HRCandidateManagement.Models.Candidate", null)
                        .WithMany()
                        .HasForeignKey("CandidatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HRCandidateManagement.Models.Skill", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
