using Kanban.Application.DTOs.Tareas;

namespace Kanban.Application.Services.Tareas;

public interface ITareaService
{
    Task<IReadOnlyList<TareaDto>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default);
    Task<TareaDto> CrearAsync(Guid proyectoId, CrearTareaRequest request, CancellationToken ct = default);
    Task<TareaDto> ActualizarAsync(Guid proyectoId, Guid tareaId, ActualizarTareaRequest request, CancellationToken ct = default);
    Task EliminarAsync(Guid proyectoId, Guid tareaId, CancellationToken ct = default);
    Task<TareaDto> MoverAsync(Guid proyectoId, Guid tareaId, MoverTareaRequest request, CancellationToken ct = default);
}
