namespace Application.Commands.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password);
