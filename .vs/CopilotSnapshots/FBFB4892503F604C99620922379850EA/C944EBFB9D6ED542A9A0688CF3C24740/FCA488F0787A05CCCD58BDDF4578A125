using HRCandidateManagement.DTOs;

namespace HRCandidateManagement.Services;

public interface ICandidateService
{
    Task<IEnumerable<CandidateResponseDto>> GetAllAsync();
    Task<CandidateResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<CandidateResponseDto>> SearchAsync(string? name, IEnumerable<string>? skillNames);
    Task<CandidateResponseDto> AddAsync(CandidateCreateDto candidateCreateDto);
    Task<CandidateResponseDto?> UpdateAsync(int id, CandidateUpdateDto candidateUpdateDto);
    Task<bool> DeleteAsync(int id);
    Task<CandidateResponseDto?> AddSkillToCandidateAsync(int candidateId, int skillId);
    Task<CandidateResponseDto?> RemoveSkillFromCandidateAsync(int candidateId, int skillId);
}
