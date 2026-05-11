using Microsoft.AspNetCore.Mvc;
using HRCandidateManagement.DTOs;
using HRCandidateManagement.Services;

namespace HRCandidateManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidateResponseDto>>> GetAll()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidateResponseDto>> GetById(int id)
    {
        var candidate = await _candidateService.GetByIdAsync(id);
        if (candidate == null)
            return NotFound();
        return Ok(candidate);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CandidateResponseDto>>> Search([FromQuery] string? name, [FromQuery(Name = "skills")] string? skillsQuery)
    {
        IEnumerable<string>? skillNames = null;
        if (!string.IsNullOrWhiteSpace(skillsQuery))
        {
            skillNames = skillsQuery.Split(char.Parse(","), StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim());
        }

        var candidates = await _candidateService.SearchAsync(name, skillNames);
        return Ok(candidates);
    }

    [HttpPost]
    public async Task<ActionResult<CandidateResponseDto>> Add([FromBody] CandidateCreateDto candidateCreateDto)
    {
        try
        {
            var candidate = await _candidateService.AddAsync(candidateCreateDto);
            return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CandidateResponseDto>> Update(int id, [FromBody] CandidateUpdateDto candidateUpdateDto)
    {
        try
        {
            var candidate = await _candidateService.UpdateAsync(id, candidateUpdateDto);
            if (candidate == null)
                return NotFound();
            return Ok(candidate);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _candidateService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{candidateId}/skills/{skillId}")]
    public async Task<ActionResult<CandidateResponseDto>> AddSkillToCandidate(int candidateId, int skillId)
    {
        var candidate = await _candidateService.AddSkillToCandidateAsync(candidateId, skillId);
        if (candidate == null)
            return NotFound();
        return Ok(candidate);
    }

    [HttpDelete("{candidateId}/skills/{skillId}")]
    public async Task<ActionResult<CandidateResponseDto>> RemoveSkillFromCandidate(int candidateId, int skillId)
    {
        var candidate = await _candidateService.RemoveSkillFromCandidateAsync(candidateId, skillId);
        if (candidate == null)
            return NotFound();
        return Ok(candidate);
    }
}
