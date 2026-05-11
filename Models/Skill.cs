namespace HRCandidateManagement.Models;

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}
