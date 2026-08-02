using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Persistence;

public interface IColumnaRepository
{
    Task<Columna?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Columna?> ObtenerConTareasAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Columna>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default);
    Task AgregarAsync(Columna columna, CancellationToken ct = default);
    void Eliminar(Columna columna);
}
