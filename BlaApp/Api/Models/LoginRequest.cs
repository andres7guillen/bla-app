namespace Api.Models;

public sealed record LoginRequest(
    string Email,
    string Password);
