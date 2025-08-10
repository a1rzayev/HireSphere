using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;
using HireSphere.Core.Enums;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationController : ControllerBase
{
    private readonly IJobApplicationEfCoreRepository _jobApplicationRepository;

    public JobApplicationController(IJobApplicationEfCoreRepository jobApplicationRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetAll()
    {
        var jobApplications = await _jobApplicationRepository.GetAllAsync();
        return Ok(jobApplications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication>> GetById(Guid id)
    {
        var jobApplication = await _jobApplicationRepository.GetByIdAsync(id);
        if (jobApplication == null) return NotFound();
        return Ok(jobApplication);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApplication)
    {
        await _jobApplicationRepository.AddAsync(jobApplication);
        return CreatedAtAction(nameof(GetById), new { id = jobApplication.Id }, jobApplication);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, JobApplication jobApplication)
    {
        var existingApplication = await _jobApplicationRepository.GetByIdAsync(id);
        if (existingApplication == null) return NotFound();
        
        existingApplication.ResumeUrl = jobApplication.ResumeUrl;
        existingApplication.CoverLetter = jobApplication.CoverLetter;
        existingApplication.Status = jobApplication.Status;
        
        await _jobApplicationRepository.UpdateAsync(existingApplication);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _jobApplicationRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("job/{jobId}")]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetByJobId(Guid jobId)
    {
        var applications = await _jobApplicationRepository.GetByJobIdAsync(jobId);
        return Ok(applications);
    }

    [HttpGet("applicant/{applicantUserId}")]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetByApplicantUserId(Guid applicantUserId)
    {
        var applications = await _jobApplicationRepository.GetByApplicantUserIdAsync(applicantUserId);
        return Ok(applications);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetByStatus(JobApplicationStatus status)
    {
        var applications = await _jobApplicationRepository.GetByStatusAsync(status);
        return Ok(applications);
    }

    [HttpGet("job/{jobId}/applicant/{applicantUserId}")]
    public async Task<ActionResult<JobApplication>> GetByJobAndApplicant(Guid jobId, Guid applicantUserId)
    {
        var application = await _jobApplicationRepository.GetByJobAndApplicantAsync(jobId, applicantUserId);
        if (application == null) return NotFound();
        return Ok(application);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] JobApplicationStatus newStatus)
    {
        var existingApplication = await _jobApplicationRepository.GetByIdAsync(id);
        if (existingApplication == null) return NotFound();
        
        try
        {
            existingApplication.ChangeStatus(newStatus);
            await _jobApplicationRepository.UpdateAsync(existingApplication);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/cover-letter")]
    public async Task<IActionResult> UpdateCoverLetter(Guid id, [FromBody] string coverLetter)
    {
        var existingApplication = await _jobApplicationRepository.GetByIdAsync(id);
        if (existingApplication == null) return NotFound();
        
        try
        {
            existingApplication.AddCoverLetter(coverLetter);
            await _jobApplicationRepository.UpdateAsync(existingApplication);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
