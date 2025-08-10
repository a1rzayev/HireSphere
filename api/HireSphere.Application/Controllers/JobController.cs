using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobEfCoreRepository _jobRepository;

    public JobController(IJobEfCoreRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Job>>> GetAll()
    {
        var jobs = await _jobRepository.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Job>> GetById(Guid id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<Job>> Create(Job job)
    {
        await _jobRepository.AddAsync(job);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Job job)
    {
        var existingJob = await _jobRepository.GetByIdAsync(id);
        if (existingJob == null) return NotFound();
        
        existingJob.Title = job.Title;
        existingJob.Description = job.Description;
        existingJob.Requirements = job.Requirements;
        existingJob.SalaryFrom = job.SalaryFrom;
        existingJob.SalaryTo = job.SalaryTo;
        existingJob.Location = job.Location;
        existingJob.JobType = job.JobType;
        existingJob.IsRemote = job.IsRemote;
        existingJob.CategoryId = job.CategoryId;
        existingJob.Tags = job.Tags;
        existingJob.ExpiresAt = job.ExpiresAt;
        existingJob.IsActive = job.IsActive;
        
        await _jobRepository.UpdateAsync(existingJob);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _jobRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<Job>>> GetByCompany(Guid companyId)
    {
        var jobs = await _jobRepository.GetByCompanyIdAsync(companyId);
        return Ok(jobs);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Job>>> GetByCategory(Guid categoryId)
    {
        var jobs = await _jobRepository.GetByCategoryIdAsync(categoryId);
        return Ok(jobs);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Job>>> GetActiveJobs()
    {
        var jobs = await _jobRepository.GetActiveJobsAsync();
        return Ok(jobs);
    }
}
