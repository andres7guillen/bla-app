using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<string>>;
