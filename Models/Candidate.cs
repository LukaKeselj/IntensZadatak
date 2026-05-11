namespace HRCandidateManagement.Models;

public class Candidate
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? ContactNumber { get; set; }
    public required string Email { get; set; }
    
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
