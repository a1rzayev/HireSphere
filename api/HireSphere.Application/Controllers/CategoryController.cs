using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryEfCoreRepository _categoryRepository;

    public CategoryController(ICategoryEfCoreRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category.Name) || category.Name.Trim().Length < 2 || category.Name.Trim().Length > 100)
            {
                return BadRequest(new { error = "Category name must be between 2 and 100 characters" });
            }

            var existingCategory = await _categoryRepository.GetByNameAsync(category.Name);
            if (existingCategory != null)
            {
                return BadRequest(new { error = "Category with this name already exists" });
            }

            await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Category category)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null) return NotFound();
        
        try
        {
            var categoryWithSameName = await _categoryRepository.GetByNameAsync(category.Name);
            if (categoryWithSameName != null && categoryWithSameName.Id != id)
            {
                return BadRequest(new { error = "Category with this name already exists" });
            }

            existingCategory.UpdateName(category.Name);
            
            await _categoryRepository.UpdateAsync(existingCategory);
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
        await _categoryRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Category>> GetByName(string name)
    {
        var category = await _categoryRepository.GetByNameAsync(name);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Category>> GetBySlug(string slug)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpGet("search/{name}")]
    public async Task<ActionResult<IEnumerable<Category>>> SearchByName(string name)
    {
        var categories = await _categoryRepository.GetByNameContainsAsync(name);
        return Ok(categories);
    }
}
