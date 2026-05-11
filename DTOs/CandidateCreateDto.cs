namespace HRCandidateManagement.DTOs;

public class CandidateCreateDto
{
    public required string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? ContactNumber { get; set; }
    public required string Email { get; set; }
}
