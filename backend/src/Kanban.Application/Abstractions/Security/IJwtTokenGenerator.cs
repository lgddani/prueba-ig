using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Security;

public record TokenGenerado(string Token, DateTime ExpiraUtc);

public interface IJwtTokenGenerator
{
    TokenGenerado GenerarToken(Usuario usuario);
}
