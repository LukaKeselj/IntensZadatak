using Microsoft.EntityFrameworkCore;
using HRCandidateManagement.Data;
using HRCandidateManagement.Models;

namespace HRCandidateManagement.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly AppDbContext _context;

    public CandidateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _context.Candidates
            .Include(c => c.Skills)
            .ToListAsync();
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        return await _context.Candidates
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidate?> GetByNameAsync(string name)
    {
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.FullName.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Candidate>> SearchAsync(string? name, IEnumerable<string>? skillNames)
    {
        var query = _context.Candidates
            .Include(c => c.Skills)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.FullName.Contains(name));
        }

        if (skillNames != null && skillNames.Any())
        {
            var skillNamesList = skillNames.Select(s => s.ToLower()).ToList();
            query = query.Where(c => c.Skills
                .Any(s => skillNamesList.Contains(s.Name.ToLower())));
        }

        return await query.ToListAsync();
    }

    public async Task<Candidate> AddAsync(Candidate candidate)
    {
        var existingCandidateWithName = await _context.Candidates
            .FirstOrDefaultAsync(c => c.FullName.ToLower() == candidate.FullName.ToLower());

        if (existingCandidateWithName != null)
        {
            throw new InvalidOperationException("Candidate name already exists.");
        }

        var existingCandidateWithEmail = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email.ToLower() == candidate.Email.ToLower());

        if (existingCandidateWithEmail != null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<Candidate?> UpdateAsync(int id, Candidate candidateUpdate)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null)
            return null;

        if (!string.IsNullOrWhiteSpace(candidateUpdate.FullName) && 
            !candidateUpdate.FullName.Equals(candidate.FullName, StringComparison.OrdinalIgnoreCase))
        {
            var existingCandidateWithName = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id != id && 
                                        c.FullName.ToLower() == candidateUpdate.FullName.ToLower());

            if (existingCandidateWithName != null)
            {
                throw new InvalidOperationException("Candidate name already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(candidateUpdate.Email) && 
            !candidateUpdate.Email.Equals(candidate.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingCandidateWithEmail = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id != id && 
                                        c.Email.ToLower() == candidateUpdate.Email.ToLower());

            if (existingCandidateWithEmail != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(candidateUpdate.FullName))
            candidate.FullName = candidateUpdate.FullName;

        if (candidateUpdate.DateOfBirth != default)
            candidate.DateOfBirth = candidateUpdate.DateOfBirth;

        if (!string.IsNullOrWhiteSpace(candidateUpdate.ContactNumber))
            candidate.ContactNumber = candidateUpdate.ContactNumber;

        if (!string.IsNullOrWhiteSpace(candidateUpdate.Email))
            candidate.Email = candidateUpdate.Email;

        _context.Candidates.Update(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null)
            return false;

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Candidate?> AddSkillToCandidateAsync(int candidateId, int skillId)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == candidateId);

        if (candidate == null)
            return null;

        var skill = await _context.Skills.FindAsync(skillId);
        if (skill == null)
            return null;

        if (!candidate.Skills.Any(s => s.Id == skillId))
        {
            candidate.Skills.Add(skill);
            await _context.SaveChangesAsync();
        }

        return candidate;
    }

    public async Task<Candidate?> RemoveSkillFromCandidateAsync(int candidateId, int skillId)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == candidateId);

        if (candidate == null)
            return null;

        var skill = candidate.Skills.FirstOrDefault(s => s.Id == skillId);
        if (skill != null)
        {
            candidate.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }

        return candidate;
    }
}
