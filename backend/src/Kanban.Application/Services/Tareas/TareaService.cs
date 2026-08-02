using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Abstractions.Realtime;
using Kanban.Application.DTOs.Tareas;
using Kanban.Application.Exceptions;
using Kanban.Domain.Entities;
using Kanban.Domain.Services;

namespace Kanban.Application.Services.Tareas;

public class TareaService : ITareaService
{
    private readonly ITareaRepository _tareas;
    private readonly IColumnaRepository _columnas;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBoardNotifier _notifier;

    public TareaService(
        ITareaRepository tareas,
        IColumnaRepository columnas,
        IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork,
        IBoardNotifier notifier)
    {
        _tareas = tareas;
        _columnas = columnas;
        _usuarios = usuarios;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task<IReadOnlyList<TareaDto>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default)
    {
        var tareas = await _tareas.ListarPorProyectoAsync(proyectoId, ct);
        var usuarios = await _usuarios.ListarAsync(ct);
        var nombresPorId = usuarios.ToDictionary(u => u.Id, u => u.Nombre);

        return tareas.Select(t => MapToDto(t, nombresPorId)).ToList();
    }

    public async Task<TareaDto> CrearAsync(Guid proyectoId, CrearTareaRequest request, CancellationToken ct = default)
    {
        var columna = await ObtenerColumnaDelProyectoAsync(proyectoId, request.ColumnaId, ct);
        var tareasColumna = await _tareas.ListarPorColumnaOrdenadoAsync(columna.Id, ct);

        var orden = OrdenTareaCalculator.CalcularOrdenAlFinal(tareasColumna.Count);
        var tarea = Tarea.Crear(request.Titulo, request.Descripcion, request.Prioridad, request.ResponsableId, columna.Id, orden);

        await _tareas.AgregarAsync(tarea, ct);
        await _unitOfWork.GuardarCambiosAsync(ct);

        var dto = await MapToDtoAsync(tarea, ct);
        await _notifier.TareaCreadaAsync(proyectoId, dto, ct);
        return dto;
    }

    public async Task<TareaDto> ActualizarAsync(Guid proyectoId, Guid tareaId, ActualizarTareaRequest request, CancellationToken ct = default)
    {
        var tarea = await ObtenerTareaDelProyectoAsync(proyectoId, tareaId, ct);
        tarea.Actualizar(request.Titulo, request.Descripcion, request.Prioridad, request.ResponsableId);
        await _unitOfWork.GuardarCambiosAsync(ct);

        var dto = await MapToDtoAsync(tarea, ct);
        await _notifier.TareaActualizadaAsync(proyectoId, dto, ct);
        return dto;
    }

    public async Task EliminarAsync(Guid proyectoId, Guid tareaId, CancellationToken ct = default)
    {
        var tarea = await ObtenerTareaDelProyectoAsync(proyectoId, tareaId, ct);
        var columnaId = tarea.ColumnaId;

        _tareas.Eliminar(tarea);
        await _unitOfWork.GuardarCambiosAsync(ct);

        await _notifier.TareaEliminadaAsync(proyectoId, tareaId, columnaId, ct);
    }

    public async Task<TareaDto> MoverAsync(Guid proyectoId, Guid tareaId, MoverTareaRequest request, CancellationToken ct = default)
    {
        var tarea = await ObtenerTareaDelProyectoAsync(proyectoId, tareaId, ct);
        var columnaOrigenId = tarea.ColumnaId;
        var columnaDestino = await ObtenerColumnaDelProyectoAsync(proyectoId, request.ColumnaDestinoId, ct);

        var tareasDestino = (await _tareas.ListarPorColumnaOrdenadoAsync(columnaDestino.Id, ct))
            .Where(t => t.Id != tareaId)
            .ToList();

        var posicion = Math.Clamp(request.PosicionDestino, 0, tareasDestino.Count);
        long? ordenAnterior = posicion > 0 ? tareasDestino[posicion - 1].Orden : null;
        long? ordenSiguiente = posicion < tareasDestino.Count ? tareasDestino[posicion].Orden : null;

        var nuevoOrden = OrdenTareaCalculator.CalcularNuevaPosicion(ordenAnterior, ordenSiguiente);

        if (nuevoOrden is null)
        {
            var idsEnOrden = tareasDestino.Select(t => t.Id).ToList();
            idsEnOrden.Insert(posicion, tareaId);

            var reasignaciones = OrdenTareaCalculator.Rebalancear(idsEnOrden);
            var mapaTareas = tareasDestino.ToDictionary(t => t.Id);
            mapaTareas[tareaId] = tarea;

            foreach (var (id, orden) in reasignaciones)
            {
                if (id == tareaId) continue;
                mapaTareas[id].ReasignarOrden(orden);
            }

            nuevoOrden = reasignaciones.First(r => r.TareaId == tareaId).NuevoOrden;
        }

        tarea.MoverA(columnaDestino.Id, nuevoOrden.Value);
        await _unitOfWork.GuardarCambiosAsync(ct);

        var dto = await MapToDtoAsync(tarea, ct);
        await _notifier.TareaMovidaAsync(proyectoId, dto, columnaOrigenId, ct);
        return dto;
    }

    private async Task<Columna> ObtenerColumnaDelProyectoAsync(Guid proyectoId, Guid columnaId, CancellationToken ct)
    {
        var columna = await _columnas.ObtenerPorIdAsync(columnaId, ct)
            ?? throw new NotFoundException(nameof(Columna), columnaId);

        if (columna.ProyectoId != proyectoId)
            throw new NotFoundException(nameof(Columna), columnaId);

        return columna;
    }

    private async Task<Tarea> ObtenerTareaDelProyectoAsync(Guid proyectoId, Guid tareaId, CancellationToken ct)
    {
        var tarea = await _tareas.ObtenerPorIdAsync(tareaId, ct)
            ?? throw new NotFoundException(nameof(Tarea), tareaId);

        var columna = await _columnas.ObtenerPorIdAsync(tarea.ColumnaId, ct);
        if (columna is null || columna.ProyectoId != proyectoId)
            throw new NotFoundException(nameof(Tarea), tareaId);

        return tarea;
    }

    private async Task<TareaDto> MapToDtoAsync(Tarea tarea, CancellationToken ct)
    {
        string? nombreResponsable = null;
        if (tarea.ResponsableId is { } responsableId)
        {
            var usuario = await _usuarios.ObtenerPorIdAsync(responsableId, ct);
            nombreResponsable = usuario?.Nombre;
        }

        return MapToDto(tarea, nombreResponsable);
    }

    private static TareaDto MapToDto(Tarea tarea, IReadOnlyDictionary<Guid, string> nombresPorId)
        => MapToDto(tarea, tarea.ResponsableId is { } id && nombresPorId.TryGetValue(id, out var n) ? n : null);

    private static TareaDto MapToDto(Tarea tarea, string? nombreResponsable) => new(
        tarea.Id,
        tarea.Titulo,
        tarea.Descripcion,
        tarea.Prioridad,
        tarea.ResponsableId,
        nombreResponsable,
        tarea.ColumnaId,
        tarea.Orden,
        tarea.FechaCreacion);
}
