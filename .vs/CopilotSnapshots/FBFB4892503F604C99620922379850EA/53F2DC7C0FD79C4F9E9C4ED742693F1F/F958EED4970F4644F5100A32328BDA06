using Microsoft.AspNetCore.Mvc;
using HRCandidateManagement.DTOs;
using HRCandidateManagement.Services;

namespace HRCandidateManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillResponseDto>>> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Ok(skills);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SkillResponseDto>> GetById(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        if (skill == null)
            return NotFound();
        return Ok(skill);
    }

    [HttpPost]
    public async Task<ActionResult<SkillResponseDto>> Add([FromBody] SkillCreateDto skillCreateDto)
    {
        try
        {
            var skill = await _skillService.AddAsync(skillCreateDto);
            return CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _skillService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
