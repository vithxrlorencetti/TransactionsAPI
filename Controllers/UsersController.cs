using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;
using TransactionsAPI.Models;

namespace TransactionsAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Returns a user by their unique identifier.")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(Summary = "Get all users", Description = "Returns all users with pagination.")]
    [ProducesResponseType(typeof(PagedResult<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _userService.GetAllAsync(page, pageSize);
        return Ok(users);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user", Description = "Creates a new user with name, email, password and adress based on postal code.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] UserCreateDTO dto)
    {
        var result = await _userService.RegisterAsync(dto.Name, dto.Email, dto.Password, dto.PostalCode);

        return Ok(new
        {
            message = $"User [{dto.Name}] successfully registered with the address: {result}"
        });
    }

    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Disable user", Description = "Disables a user if they have no transactions.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Disable(int id)
    {
        var result = await _userService.DisableByIdAsync(id);

        return Ok(new
        {
            message = $"User [{result}] successfully disabled"
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login user", Description = "Returns a JWT token for a valid email and password. Use it in the Authorization header as: Bearer {token}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var token = await _userService.LoginAsync(dto.Email, dto.Password);
        return Ok(token);
    }
}
