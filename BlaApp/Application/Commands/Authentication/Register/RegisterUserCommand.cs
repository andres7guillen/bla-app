using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Authentication.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password)
    : IRequest<Result<Guid>>;
