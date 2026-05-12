using HRCandidateManagement.Models;

namespace HRCandidateManagement.Repositories;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<IEnumerable<Candidate>> SearchAsync(string? name, IEnumerable<string>? skillNames);
    Task<Candidate?> UpdateAsync(int id, Candidate candidate);
    Task<Candidate?> AddSkillToCandidateAsync(int candidateId, int skillId);
    Task<Candidate?> RemoveSkillFromCandidateAsync(int candidateId, int skillId);
}
