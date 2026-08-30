using Api.Models;
using Application.Commands.Authentication.Login;
using Application.Commands.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterUserCommand(
                request.Email,
                request.Password),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                userId = result.Value
            });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginCommand(
                request.Email,
                request.Password),
            cancellationToken);

        if (result.IsFailure)
            return Unauthorized(result.Error);

        return Ok(new
        {
            token = result.Value
        });
    }
}