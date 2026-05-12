using Xunit;
using Microsoft.EntityFrameworkCore;
using HRCandidateManagement.Data;
using HRCandidateManagement.Models;
using HRCandidateManagement.Repositories;

namespace HRCandidateManagement.Tests;

public class CandidateRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Candidate CreateTestCandidate(string fullName = "John Doe", 
        string email = "john@example.com")
    {
        return new Candidate
        {
            FullName = fullName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            ContactNumber = "123456789",
            Email = email
        };
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithMultipleCandidates_ReturnsAllCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidates = new List<Candidate>
        {
            CreateTestCandidate("John Doe", "john@example.com"),
            CreateTestCandidate("Jane Smith", "jane@example.com"),
            CreateTestCandidate("Bob Johnson", "bob@example.com")
        };

        context.Candidates.AddRange(candidates);
        await context.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.FullName == "John Doe");
        Assert.Contains(result, c => c.FullName == "Jane Smith");
        Assert.Contains(result, c => c.FullName == "Bob Johnson");
    }

    [Fact]
    public async Task GetAllAsync_WithNoCandidates_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var result = await repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_IncludesSkills()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        Assert.NotEmpty(result);
        var retrievedCandidate = result.First();
        Assert.NotEmpty(retrievedCandidate.Skills);
        Assert.Contains(retrievedCandidate.Skills, s => s.Name == "C#");
    }

    #endregion

    #region GetByNameAsync Tests

    [Fact]
    public async Task GetByNameAsync_WithExactName_ReturnsCandidate()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate("John Doe");
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetByNameAsync("John Doe");

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetByNameAsync_WithDifferentCase_ReturnsCandidate()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate("John Doe");
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetByNameAsync("john doe");

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistentName_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate("John Doe");
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetByNameAsync("NonExistent");

        Assert.Null(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectCandidate()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(candidate.Id);

        Assert.NotNull(result);
        Assert.Equal(candidate.Id, result.Id);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesSkills()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "JavaScript" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(candidate.Id);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Skills);
        Assert.Contains(result.Skills, s => s.Name == "JavaScript");
    }

    #endregion

    #region SearchAsync Tests

    [Fact]
    public async Task SearchAsync_WithNameFilter_ReturnsMatchingCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidates = new List<Candidate>
        {
            CreateTestCandidate("John Doe", "john@example.com"),
            CreateTestCandidate("Jane Smith", "jane@example.com")
        };

        context.Candidates.AddRange(candidates);
        await context.SaveChangesAsync();

        var result = await repository.SearchAsync("John", null);

        Assert.Single(result);
        Assert.Equal("John Doe", result.First().FullName);
    }

    [Fact]
    public async Task SearchAsync_WithSkillFilter_ReturnsMatchingCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.SearchAsync(null, new List<string> { "C#" });

        Assert.NotEmpty(result);
        Assert.Contains(result, c => c.FullName == "John Doe");
    }

    [Fact]
    public async Task SearchAsync_WithNameAndSkillFilters_ReturnsMatchingCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill1 = new Skill { Name = "C#" };
        var skill2 = new Skill { Name = "JavaScript" };
        context.Skills.AddRange(skill1, skill2);
        await context.SaveChangesAsync();

        var candidate1 = CreateTestCandidate("John Doe", "john@example.com");
        candidate1.Skills.Add(skill1);

        var candidate2 = CreateTestCandidate("Jane Smith", "jane@example.com");
        candidate2.Skills.Add(skill2);

        context.Candidates.AddRange(candidate1, candidate2);
        await context.SaveChangesAsync();

        var result = await repository.SearchAsync("John", new List<string> { "C#" });

        Assert.Single(result);
        Assert.Equal("John Doe", result.First().FullName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.SearchAsync("NonExistent", null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithSkillFilterCaseInsensitive_ReturnsMatchingCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.SearchAsync(null, new List<string> { "c#" });

        Assert.NotEmpty(result);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidCandidate_CreatesSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();

        var result = await repository.AddAsync(candidate);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("John Doe", result.FullName);

        var savedCandidate = await context.Candidates.FindAsync(result.Id);
        Assert.NotNull(savedCandidate);
        Assert.Equal("John Doe", savedCandidate.FullName);
    }

    [Fact]
    public async Task AddAsync_WithMultipleCandidates_CreatesAllSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate1 = CreateTestCandidate("John Doe", "john@example.com");
        var candidate2 = CreateTestCandidate("Jane Smith", "jane@example.com");

        var result1 = await repository.AddAsync(candidate1);
        var result2 = await repository.AddAsync(candidate2);

        Assert.True(result1.Id > 0);
        Assert.True(result2.Id > 0);
        Assert.NotEqual(result1.Id, result2.Id);

        var allCandidates = await repository.GetAllAsync();
        Assert.Equal(2, allCandidates.Count());
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var updatedCandidate = new Candidate
        {
            FullName = "Updated Name",
            Email = "updated@example.com",
            DateOfBirth = new DateOnly(1995, 5, 5),
            ContactNumber = "987654321"
        };

        var result = await repository.UpdateAsync(candidate.Id, updatedCandidate);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal("updated@example.com", result.Email);

        var verifyUpdate = await context.Candidates.FindAsync(candidate.Id);
        Assert.Equal("Updated Name", verifyUpdate!.FullName);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidateUpdate = CreateTestCandidate("Updated", "updated@example.com");

        var result = await repository.UpdateAsync(999, candidateUpdate);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithPartialUpdate_UpdatesOnlyProvidedFields()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate("John Doe", "john@example.com");
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var originalContactNumber = candidate.ContactNumber;

        var partialUpdate = new Candidate
        {
            FullName = "Updated Name",
            Email = "",
            DateOfBirth = default,
            ContactNumber = null
        };

        var result = await repository.UpdateAsync(candidate.Id, partialUpdate);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal(originalContactNumber, result.ContactNumber);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.DeleteAsync(candidate.Id);

        Assert.True(result);

        var deletedCandidate = await context.Candidates.FindAsync(candidate.Id);
        Assert.Null(deletedCandidate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var result = await repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DoesNotAffectOtherCandidates()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate1 = CreateTestCandidate("John Doe", "john@example.com");
        var candidate2 = CreateTestCandidate("Jane Smith", "jane@example.com");
        context.Candidates.AddRange(candidate1, candidate2);
        await context.SaveChangesAsync();

        await repository.DeleteAsync(candidate1.Id);

        var remainingCandidates = await repository.GetAllAsync();
        Assert.Single(remainingCandidates);
        Assert.Equal("Jane Smith", remainingCandidates.First().FullName);
    }

    #endregion

    #region Skill Management Tests

    [Fact]
    public async Task AddSkillToCandidateAsync_WithValidIds_AddsSkillSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.AddSkillToCandidateAsync(candidate.Id, skill.Id);

        Assert.NotNull(result);
        Assert.Contains(result.Skills, s => s.Name == "C#");
    }

    [Fact]
    public async Task AddSkillToCandidateAsync_WithInvalidCandidateId_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var result = await repository.AddSkillToCandidateAsync(999, skill.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddSkillToCandidateAsync_WithInvalidSkillId_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.AddSkillToCandidateAsync(candidate.Id, 999);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddSkillToCandidateAsync_WithDuplicateSkill_DoesNotAddDuplicate()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.AddSkillToCandidateAsync(candidate.Id, skill.Id);

        Assert.NotNull(result);
        Assert.Single(result.Skills.Where(s => s.Name == "C#"));
    }

    [Fact]
    public async Task RemoveSkillFromCandidateAsync_WithValidIds_RemovesSkillSuccessfully()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate();
        candidate.Skills.Add(skill);
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.RemoveSkillFromCandidateAsync(candidate.Id, skill.Id);

        Assert.NotNull(result);
        Assert.Empty(result.Skills);
    }

    [Fact]
    public async Task RemoveSkillFromCandidateAsync_WithInvalidCandidateId_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var result = await repository.RemoveSkillFromCandidateAsync(999, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveSkillFromCandidateAsync_WithNonExistentSkill_DoesNotThrow()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var candidate = CreateTestCandidate();
        context.Candidates.Add(candidate);
        await context.SaveChangesAsync();

        var result = await repository.RemoveSkillFromCandidateAsync(candidate.Id, 999);

        Assert.NotNull(result);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteWorkflow_CreateSearchUpdateDelete_SuccessfullyManipulatesCandidate()
    {
        using var context = CreateContext();
        var repository = new CandidateRepository(context);

        var skill = new Skill { Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var candidate = CreateTestCandidate("John Doe", "john@example.com");

        var createdCandidate = await repository.AddAsync(candidate);

        var searchResult = await repository.SearchAsync("John", null);
        Assert.Single(searchResult);

        var candidateWithSkill = await repository.AddSkillToCandidateAsync(createdCandidate.Id, skill.Id);
        Assert.NotNull(candidateWithSkill);
        Assert.Single(candidateWithSkill.Skills);

        var updatedCandidate = new Candidate
        {
            FullName = "Updated John Doe",
            Email = "updated@example.com"
        };
        var updateResult = await repository.UpdateAsync(createdCandidate.Id, updatedCandidate);
        Assert.NotNull(updateResult);
        Assert.Equal("Updated John Doe", updateResult.FullName);

        var candidateWithoutSkill = await repository.RemoveSkillFromCandidateAsync(
            createdCandidate.Id, skill.Id);
        Assert.NotNull(candidateWithoutSkill);
        Assert.Empty(candidateWithoutSkill.Skills);

        var deleteResult = await repository.DeleteAsync(createdCandidate.Id);
        Assert.True(deleteResult);

        var deletedCandidate = await repository.GetByIdAsync(createdCandidate.Id);
        Assert.Null(deletedCandidate);
    }

    #endregion
}
