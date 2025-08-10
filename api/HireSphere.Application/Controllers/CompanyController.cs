using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyEfCoreRepository _companyRepository;

    public CompanyController(ICompanyEfCoreRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetAll()
    {
        var companies = await _companyRepository.GetAllAsync();
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetById(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotFound();
        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<Company>> Create(Company company)
    {
        try
        {
            company.ValidateCompany();
            await _companyRepository.AddAsync(company);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Company company)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(id);
        if (existingCompany == null) return NotFound();
        
        try
        {
            existingCompany.Name = company.Name;
            existingCompany.Description = company.Description;
            existingCompany.Website = company.Website;
            existingCompany.Location = company.Location;
            
            existingCompany.ValidateCompany();
            await _companyRepository.UpdateAsync(existingCompany);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _companyRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("owner/{ownerUserId}")]
    public async Task<ActionResult<IEnumerable<Company>>> GetByOwnerUserId(Guid ownerUserId)
    {
        var companies = await _companyRepository.GetByOwnerUserIdAsync(ownerUserId);
        return Ok(companies);
    }

    [HttpGet("search/name/{name}")]
    public async Task<ActionResult<IEnumerable<Company>>> GetByName(string name)
    {
        var companies = await _companyRepository.GetByNameAsync(name);
        return Ok(companies);
    }

    [HttpGet("search/location/{location}")]
    public async Task<ActionResult<IEnumerable<Company>>> GetByLocation(string location)
    {
        var companies = await _companyRepository.GetByLocationAsync(location);
        return Ok(companies);
    }

    [HttpPut("{id}/logo")]
    public async Task<IActionResult> UpdateLogo(Guid id, [FromBody] string? logoUrl)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(id);
        if (existingCompany == null) return NotFound();
        
        try
        {
            existingCompany.UpdateLogoUrl(logoUrl);
            await _companyRepository.UpdateAsync(existingCompany);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
