using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserEfCoreRepository _userRepository;

    public UserController(IUserEfCoreRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        await _userRepository.AddAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return NotFound();
        
        existingUser.Name = user.Name;
        existingUser.Surname = user.Surname;
        existingUser.Email = user.Email;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.Role = user.Role;
        existingUser.Phone = user.Phone;
        
        await _userRepository.UpdateAsync(existingUser);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        return NoContent();
    }
}
