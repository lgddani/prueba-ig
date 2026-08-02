using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.DTOs.Columnas;
using Kanban.Application.Exceptions;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services.Columnas;

public class ColumnaService : IColumnaService
{
    private readonly IColumnaRepository _columnas;
    private readonly IProyectoRepository _proyectos;
    private readonly IUnitOfWork _unitOfWork;

    public ColumnaService(IColumnaRepository columnas, IProyectoRepository proyectos, IUnitOfWork unitOfWork)
    {
        _columnas = columnas;
        _proyectos = proyectos;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ColumnaDto>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default)
    {
        var columnas = await _columnas.ListarPorProyectoAsync(proyectoId, ct);
        return columnas.OrderBy(c => c.Orden).Select(MapToDto).ToList();
    }

    public async Task<ColumnaDto> CrearAsync(Guid proyectoId, CrearColumnaRequest request, CancellationToken ct = default)
    {
        _ = await _proyectos.ObtenerPorIdAsync(proyectoId, ct) ?? throw new NotFoundException(nameof(Proyecto), proyectoId);

        var existentes = await _columnas.ListarPorProyectoAsync(proyectoId, ct);
        var columna = Columna.Crear(request.Nombre, existentes.Count, proyectoId);

        await _columnas.AgregarAsync(columna, ct);
        await _unitOfWork.GuardarCambiosAsync(ct);

        return MapToDto(columna);
    }

    public async Task<ColumnaDto> ActualizarAsync(Guid proyectoId, Guid columnaId, ActualizarColumnaRequest request, CancellationToken ct = default)
    {
        var columna = await ObtenerDelProyectoAsync(proyectoId, columnaId, ct);
        columna.Renombrar(request.Nombre);
        await _unitOfWork.GuardarCambiosAsync(ct);
        return MapToDto(columna);
    }

    public async Task EliminarAsync(Guid proyectoId, Guid columnaId, CancellationToken ct = default)
    {
        var columna = await _columnas.ObtenerConTareasAsync(columnaId, ct)
            ?? throw new NotFoundException(nameof(Columna), columnaId);

        if (columna.ProyectoId != proyectoId)
            throw new NotFoundException(nameof(Columna), columnaId);

        if (!columna.PuedeEliminarse())
            throw new BusinessRuleException("No se puede eliminar una columna que contiene tareas.");

        _columnas.Eliminar(columna);
        await _unitOfWork.GuardarCambiosAsync(ct);
    }

    public async Task ReordenarAsync(Guid proyectoId, ReordenarColumnasRequest request, CancellationToken ct = default)
    {
        var columnas = await _columnas.ListarPorProyectoAsync(proyectoId, ct);
        var mapa = columnas.ToDictionary(c => c.Id);

        for (var i = 0; i < request.ColumnaIdsEnOrden.Count; i++)
        {
            if (mapa.TryGetValue(request.ColumnaIdsEnOrden[i], out var columna))
                columna.CambiarOrden(i);
        }

        await _unitOfWork.GuardarCambiosAsync(ct);
    }

    private async Task<Columna> ObtenerDelProyectoAsync(Guid proyectoId, Guid columnaId, CancellationToken ct)
    {
        var columna = await _columnas.ObtenerPorIdAsync(columnaId, ct)
            ?? throw new NotFoundException(nameof(Columna), columnaId);

        if (columna.ProyectoId != proyectoId)
            throw new NotFoundException(nameof(Columna), columnaId);

        return columna;
    }

    private static ColumnaDto MapToDto(Columna columna) => new(
        columna.Id, columna.Nombre, columna.Orden, columna.ProyectoId, columna.Tareas.Count);
}
