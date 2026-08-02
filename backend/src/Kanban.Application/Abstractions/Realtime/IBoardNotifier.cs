using Kanban.Application.DTOs.Tareas;

namespace Kanban.Application.Abstractions.Realtime;

/// <summary>
/// Puerto de salida hacia el canal de tiempo real.
/// </summary>
public interface IBoardNotifier
{
    Task TareaCreadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken ct = default);
    Task TareaActualizadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken ct = default);
    Task TareaEliminadaAsync(Guid proyectoId, Guid tareaId, Guid columnaId, CancellationToken ct = default);
    Task TareaMovidaAsync(Guid proyectoId, TareaDto tarea, Guid columnaOrigenId, CancellationToken ct = default);
}
