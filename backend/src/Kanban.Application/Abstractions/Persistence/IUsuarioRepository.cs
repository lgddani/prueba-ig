using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Persistence;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken ct = default);
}
