using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Persistence;

public interface ITareaRepository
{
    Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Tarea>> ListarPorColumnaOrdenadoAsync(Guid columnaId, CancellationToken ct = default);
    Task<IReadOnlyList<Tarea>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default);
    Task AgregarAsync(Tarea tarea, CancellationToken ct = default);
    void Eliminar(Tarea tarea);
}
