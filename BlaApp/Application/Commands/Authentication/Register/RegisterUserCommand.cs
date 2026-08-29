namespace Application.Commands.Authentication.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password);
