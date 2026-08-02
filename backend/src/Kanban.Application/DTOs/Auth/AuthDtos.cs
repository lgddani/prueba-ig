using System.ComponentModel.DataAnnotations;

namespace Kanban.Application.DTOs.Auth;

public record LoginRequest(
    [Required, EmailAddress] string Correo,
    [Required] string Password
);

public record LoginResponse(string Token, DateTime ExpiraUtc, UsuarioDto Usuario);

public record UsuarioDto(Guid Id, string Nombre, string Correo);
