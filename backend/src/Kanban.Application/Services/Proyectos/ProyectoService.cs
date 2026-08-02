using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Common;
using Kanban.Application.DTOs.Proyectos;
using Kanban.Application.Exceptions;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services.Proyectos;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _proyectos;
    private readonly IUnitOfWork _unitOfWork;

    public ProyectoService(IProyectoRepository proyectos, IUnitOfWork unitOfWork)
    {
        _proyectos = proyectos;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProyectoDto>> ListarAsync(int pagina, int tamanoPagina, string? filtroNombre, CancellationToken ct = default)
    {
        pagina = pagina < 1 ? 1 : pagina;
        tamanoPagina = tamanoPagina is < 1 or > 100 ? 10 : tamanoPagina;

        var (items, total) = await _proyectos.ListarPaginadoAsync(pagina, tamanoPagina, filtroNombre, ct);

        return new PagedResult<ProyectoDto>
        {
            Items = items.Select(MapToDto).ToList(),
            PaginaActual = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<ProyectoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var proyecto = await _proyectos.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Proyecto), id);
        return MapToDto(proyecto);
    }

    public async Task<ProyectoDto> CrearAsync(CrearProyectoRequest request, CancellationToken ct = default)
    {
        var proyecto = Proyecto.Crear(request.Nombre, request.Descripcion, request.FechaInicio, request.FechaFinPrevista, request.Estado);

        await _proyectos.AgregarAsync(proyecto, ct);
        await _unitOfWork.GuardarCambiosAsync(ct);

        return MapToDto(proyecto);
    }

    public async Task<ProyectoDto> ActualizarAsync(Guid id, ActualizarProyectoRequest request, CancellationToken ct = default)
    {
        var proyecto = await _proyectos.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Proyecto), id);

        proyecto.Actualizar(request.Nombre, request.Descripcion, request.FechaInicio, request.FechaFinPrevista, request.Estado);
        await _unitOfWork.GuardarCambiosAsync(ct);

        return MapToDto(proyecto);
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct = default)
    {
        var proyecto = await _proyectos.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Proyecto), id);

        _proyectos.Eliminar(proyecto);
        await _unitOfWork.GuardarCambiosAsync(ct);
    }

    private static ProyectoDto MapToDto(Proyecto proyecto) => new(
        proyecto.Id,
        proyecto.Nombre,
        proyecto.Descripcion,
        proyecto.FechaInicio,
        proyecto.FechaFinPrevista,
        proyecto.Estado,
        proyecto.FechaCreacion,
        proyecto.Columnas.Count);
}
