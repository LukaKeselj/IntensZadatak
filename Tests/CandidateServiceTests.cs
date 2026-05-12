using Xunit;
using Moq;
using HRCandidateManagement.DTOs;
using HRCandidateManagement.Models;
using HRCandidateManagement.Repositories;
using HRCandidateManagement.Services;

namespace HRCandidateManagement.Tests;

public class CandidateServiceTests
{
    private readonly Mock<ICandidateRepository> _mockCandidateRepository;
    private readonly Mock<ISkillRepository> _mockSkillRepository;
    private readonly CandidateService _candidateService;

    public CandidateServiceTests()
    {
        _mockCandidateRepository = new Mock<ICandidateRepository>();
        _mockSkillRepository = new Mock<ISkillRepository>();
        _candidateService = new CandidateService(_mockCandidateRepository.Object, _mockSkillRepository.Object);
    }

    private static Candidate CreateTestCandidate(int id = 1, string fullName = "John Doe", 
        string email = "john@example.com")
    {
        return new Candidate
        {
            Id = id,
            FullName = fullName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            ContactNumber = "123456789",
            Email = email,
            Skills = new List<Skill>()
        };
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithCandidates_ReturnsAllCandidatesAsDto()
    {
        var candidates = new List<Candidate>
        {
            CreateTestCandidate(1, "John Doe", "john@example.com"),
            CreateTestCandidate(2, "Jane Smith", "jane@example.com"),
            CreateTestCandidate(3, "Bob Johnson", "bob@example.com")
        };

        _mockCandidateRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(candidates);

        var result = await _candidateService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.FullName == "John Doe");
        Assert.Contains(result, c => c.FullName == "Jane Smith");
        Assert.Contains(result, c => c.FullName == "Bob Johnson");

        _mockCandidateRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithNoCandidates_ReturnsEmptyList()
    {
        _mockCandidateRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Candidate>());

        var result = await _candidateService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
        _mockCandidateRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCandidateAsDto()
    {
        int candidateId = 1;
        var candidate = CreateTestCandidate(candidateId);

        _mockCandidateRepository.Setup(r => r.GetByIdAsync(candidateId))
            .ReturnsAsync(candidate);

        var result = await _candidateService.GetByIdAsync(candidateId);

        Assert.NotNull(result);
        Assert.Equal(candidateId, result.Id);
        Assert.Equal("John Doe", result.FullName);
        _mockCandidateRepository.Verify(r => r.GetByIdAsync(candidateId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        int invalidId = 999;
        _mockCandidateRepository.Setup(r => r.GetByIdAsync(invalidId))
            .ReturnsAsync((Candidate?)null);

        var result = await _candidateService.GetByIdAsync(invalidId);

        Assert.Null(result);
        _mockCandidateRepository.Verify(r => r.GetByIdAsync(invalidId), Times.Once);
    }

    #endregion

    #region SearchAsync Tests

    [Fact]
    public async Task SearchAsync_WithNameFilter_ReturnsMatchingCandidates()
    {
        var candidates = new List<Candidate>
        {
            CreateTestCandidate(1, "John Doe", "john@example.com")
        };

        _mockCandidateRepository.Setup(r => r.SearchAsync("John", null))
            .ReturnsAsync(candidates);

        var result = await _candidateService.SearchAsync("John", null);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result.First().FullName);
        _mockCandidateRepository.Verify(r => r.SearchAsync("John", null), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSkillsFilter_ReturnsMatchingCandidates()
    {
        var candidate = CreateTestCandidate(1);
        candidate.Skills.Add(new Skill { Id = 1, Name = "C#" });

        var candidates = new List<Candidate> { candidate };
        var skillNames = new List<string> { "C#" };

        _mockCandidateRepository.Setup(r => r.SearchAsync(null, skillNames))
            .ReturnsAsync(candidates);

        var result = await _candidateService.SearchAsync(null, skillNames);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains("C#", result.First().SkillNames);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        _mockCandidateRepository.Setup(r => r.SearchAsync("NonExistent", null))
            .ReturnsAsync(new List<Candidate>());

        var result = await _candidateService.SearchAsync("NonExistent", null);

        Assert.Empty(result);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidDto_CreatesNewCandidate()
    {
        var candidateCreateDto = new CandidateCreateDto
        {
            FullName = "John Doe",
            DateOfBirth = new DateOnly(1990, 1, 1),
            ContactNumber = "123456789",
            Email = "john@example.com"
        };

        var createdCandidate = new Candidate
        {
            Id = 1,
            FullName = candidateCreateDto.FullName,
            DateOfBirth = candidateCreateDto.DateOfBirth,
            ContactNumber = candidateCreateDto.ContactNumber,
            Email = candidateCreateDto.Email
        };

        _mockCandidateRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Candidate>());

        _mockCandidateRepository.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .ReturnsAsync(createdCandidate);

        var result = await _candidateService.AddAsync(candidateCreateDto);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("john@example.com", result.Email);
        _mockCandidateRepository.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        var candidateCreateDto = new CandidateCreateDto
        {
            FullName = "John Doe",
            Email = "john@example.com"
        };

        _mockCandidateRepository.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.AddAsync(candidateCreateDto)
        );

        Assert.Equal("Email already exists.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithExistingEmailDifferentCase_ThrowsInvalidOperationException()
    {
        var candidateCreateDto = new CandidateCreateDto
        {
            FullName = "New User",
            Email = "john@example.com"
        };

        _mockCandidateRepository.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.AddAsync(candidateCreateDto)
        );

        Assert.Equal("Email already exists.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithExistingName_ThrowsInvalidOperationException()
    {
        var candidateCreateDto = new CandidateCreateDto
        {
            FullName = "John Doe",
            Email = "newuser@example.com"
        };

        _mockCandidateRepository.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Candidate name already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.AddAsync(candidateCreateDto)
        );

        Assert.Equal("Candidate name already exists.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithExistingNameDifferentCase_ThrowsInvalidOperationException()
    {
        var candidateCreateDto = new CandidateCreateDto
        {
            FullName = "john doe",
            Email = "newuser@example.com"
        };

        _mockCandidateRepository.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Candidate name already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.AddAsync(candidateCreateDto)
        );

        Assert.Equal("Candidate name already exists.", exception.Message);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCandidateSuccessfully()
    {
        int candidateId = 1;
        var existingCandidate = CreateTestCandidate(candidateId);
        var updateDto = new CandidateUpdateDto
        {
            FullName = "Updated Name",
            Email = "updated@example.com"
        };

        var updatedCandidate = new Candidate
        {
            Id = candidateId,
            FullName = "Updated Name",
            DateOfBirth = existingCandidate.DateOfBirth,
            ContactNumber = existingCandidate.ContactNumber,
            Email = "updated@example.com"
        };

        _mockCandidateRepository.Setup(r => r.GetByIdAsync(candidateId))
            .ReturnsAsync(existingCandidate);

        _mockCandidateRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Candidate>());

        _mockCandidateRepository.Setup(r => r.GetByNameAsync("Updated Name"))
            .ReturnsAsync((Candidate?)null);

        _mockCandidateRepository.Setup(r => r.UpdateAsync(candidateId, It.IsAny<Candidate>()))
            .ReturnsAsync(updatedCandidate);

        var result = await _candidateService.UpdateAsync(candidateId, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal("updated@example.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        var updateDto = new CandidateUpdateDto { FullName = "Updated" };
        _mockCandidateRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Candidate?)null);

        var result = await _candidateService.UpdateAsync(999, updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        int candidateId = 1;
        var existingCandidate = CreateTestCandidate(candidateId, "John Doe", "john@example.com");
        var updateDto = new CandidateUpdateDto { Email = "jane@example.com" };

        _mockCandidateRepository.Setup(r => r.GetByIdAsync(candidateId))
            .ReturnsAsync(existingCandidate);

        _mockCandidateRepository.Setup(r => r.UpdateAsync(candidateId, It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.UpdateAsync(candidateId, updateDto)
        );

        Assert.Equal("Email already exists.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingName_ThrowsInvalidOperationException()
    {
        int candidateId = 1;
        var existingCandidate = CreateTestCandidate(candidateId, "John Doe");
        var updateDto = new CandidateUpdateDto { FullName = "Jane Smith" };

        _mockCandidateRepository.Setup(r => r.GetByIdAsync(candidateId))
            .ReturnsAsync(existingCandidate);

        _mockCandidateRepository.Setup(r => r.UpdateAsync(candidateId, It.IsAny<Candidate>()))
            .ThrowsAsync(new InvalidOperationException("Candidate name already exists."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _candidateService.UpdateAsync(candidateId, updateDto)
        );

        Assert.Equal("Candidate name already exists.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithSameName_UpdatesSuccessfully()
    {
        int candidateId = 1;
        var existingCandidate = CreateTestCandidate(candidateId, "John Doe");
        var updateDto = new CandidateUpdateDto { FullName = "John Doe" };

        var updatedCandidate = new Candidate
        {
            Id = candidateId,
            FullName = "John Doe",
            DateOfBirth = existingCandidate.DateOfBirth,
            ContactNumber = existingCandidate.ContactNumber,
            Email = existingCandidate.Email
        };

        _mockCandidateRepository.Setup(r => r.GetByIdAsync(candidateId))
            .ReturnsAsync(existingCandidate);

        _mockCandidateRepository.Setup(r => r.UpdateAsync(candidateId, It.IsAny<Candidate>()))
            .ReturnsAsync(updatedCandidate);

        var result = await _candidateService.UpdateAsync(candidateId, updateDto);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        _mockCandidateRepository.Verify(r => r.UpdateAsync(candidateId, It.IsAny<Candidate>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCandidateSuccessfully()
    {
        int candidateId = 1;
        _mockCandidateRepository.Setup(r => r.DeleteAsync(candidateId))
            .ReturnsAsync(true);

        var result = await _candidateService.DeleteAsync(candidateId);

        Assert.True(result);
        _mockCandidateRepository.Verify(r => r.DeleteAsync(candidateId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        int invalidId = 999;
        _mockCandidateRepository.Setup(r => r.DeleteAsync(invalidId))
            .ReturnsAsync(false);

        var result = await _candidateService.DeleteAsync(invalidId);

        Assert.False(result);
    }

    #endregion

    #region Skill Management Tests

    [Fact]
    public async Task AddSkillToCandidateAsync_WithValidIds_AddsSkillSuccessfully()
    {
        int candidateId = 1;
        int skillId = 1;
        var candidate = CreateTestCandidate(candidateId);
        candidate.Skills.Add(new Skill { Id = skillId, Name = "C#" });

        _mockCandidateRepository.Setup(r => r.AddSkillToCandidateAsync(candidateId, skillId))
            .ReturnsAsync(candidate);

        var result = await _candidateService.AddSkillToCandidateAsync(candidateId, skillId);

        Assert.NotNull(result);
        Assert.Contains("C#", result.SkillNames);
        _mockCandidateRepository.Verify(
            r => r.AddSkillToCandidateAsync(candidateId, skillId), Times.Once);
    }

    [Fact]
    public async Task AddSkillToCandidateAsync_WithInvalidCandidateId_ReturnsNull()
    {
        _mockCandidateRepository.Setup(r => r.AddSkillToCandidateAsync(999, 1))
            .ReturnsAsync((Candidate?)null);

        var result = await _candidateService.AddSkillToCandidateAsync(999, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveSkillFromCandidateAsync_WithValidIds_RemovesSkillSuccessfully()
    {
        int candidateId = 1;
        int skillId = 1;
        var candidate = CreateTestCandidate(candidateId);

        _mockCandidateRepository.Setup(r => r.RemoveSkillFromCandidateAsync(candidateId, skillId))
            .ReturnsAsync(candidate);

        var result = await _candidateService.RemoveSkillFromCandidateAsync(candidateId, skillId);

        Assert.NotNull(result);
        _mockCandidateRepository.Verify(
            r => r.RemoveSkillFromCandidateAsync(candidateId, skillId), Times.Once);
    }

    [Fact]
    public async Task RemoveSkillFromCandidateAsync_WithInvalidCandidateId_ReturnsNull()
    {
        _mockCandidateRepository.Setup(r => r.RemoveSkillFromCandidateAsync(999, 1))
            .ReturnsAsync((Candidate?)null);

        var result = await _candidateService.RemoveSkillFromCandidateAsync(999, 1);

        Assert.Null(result);
    }

    #endregion
}
