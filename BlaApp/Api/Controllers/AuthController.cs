using Api.Models;
using Application.Commands.Authentication.Login;
using Application.Commands.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;

    public AuthController(
        RegisterUserCommandHandler registerHandler,
        LoginCommandHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password);

        var result =
            await _registerHandler.Handle(command);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                userId = result.Value
            });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result =
            await _loginHandler.Handle(command);

        if (result.IsFailure)
        {
            return Unauthorized(new
            {
                error = result.Error
            });
        }

        return Ok(new
        {
            token = result.Value
        });
    }
}
