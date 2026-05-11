namespace HRCandidateManagement.DTOs;

public class CandidateUpdateDto
{
    public string? FullName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
}
