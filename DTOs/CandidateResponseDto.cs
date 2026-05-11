namespace HRCandidateManagement.DTOs;

public class CandidateResponseDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? ContactNumber { get; set; }
    public required string Email { get; set; }
    public List<string> SkillNames { get; set; } = new();
}
