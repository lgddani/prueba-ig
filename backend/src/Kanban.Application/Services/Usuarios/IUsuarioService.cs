using Kanban.Application.DTOs.Auth;

namespace Kanban.Application.Services.Usuarios;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ListarAsync(CancellationToken ct = default);
}
