using Kanban.Application.Abstractions.Realtime;
using Kanban.Application.DTOs.Tareas;
using Microsoft.AspNetCore.SignalR;

namespace Kanban.Infrastructure.Realtime;

public static class BoardEvents
{
    public const string TareaCreada = "TareaCreada";
    public const string TareaActualizada = "TareaActualizada";
    public const string TareaEliminada = "TareaEliminada";
    public const string TareaMovida = "TareaMovida";
    public const string UsuariosConectados = "UsuariosConectados";
}

public class SignalRBoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hubContext;

    public SignalRBoardNotifier(IHubContext<BoardHub> hubContext) => _hubContext = hubContext;

    public Task TareaCreadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken ct = default)
        => Grupo(proyectoId).SendAsync(BoardEvents.TareaCreada, tarea, ct);

    public Task TareaActualizadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken ct = default)
        => Grupo(proyectoId).SendAsync(BoardEvents.TareaActualizada, tarea, ct);

    public Task TareaEliminadaAsync(Guid proyectoId, Guid tareaId, Guid columnaId, CancellationToken ct = default)
        => Grupo(proyectoId).SendAsync(BoardEvents.TareaEliminada, new { tareaId, columnaId }, ct);

    public Task TareaMovidaAsync(Guid proyectoId, TareaDto tarea, Guid columnaOrigenId, CancellationToken ct = default)
        => Grupo(proyectoId).SendAsync(BoardEvents.TareaMovida, new { tarea, columnaOrigenId }, ct);

    private IClientProxy Grupo(Guid proyectoId) => _hubContext.Clients.Group(BoardHub.GroupName(proyectoId));
}
