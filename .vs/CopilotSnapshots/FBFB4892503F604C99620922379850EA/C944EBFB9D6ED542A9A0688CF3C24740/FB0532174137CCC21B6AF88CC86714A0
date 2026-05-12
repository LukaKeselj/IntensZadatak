using HRCandidateManagement.DTOs;

namespace HRCandidateManagement.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillResponseDto>> GetAllAsync();
    Task<SkillResponseDto?> GetByIdAsync(int id);
    Task<SkillResponseDto> AddAsync(SkillCreateDto skillCreateDto);
    Task<bool> DeleteAsync(int id);
}
