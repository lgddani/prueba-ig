using Kanban.Application.Common;
using Kanban.Application.DTOs.Proyectos;

namespace Kanban.Application.Services.Proyectos;

public interface IProyectoService
{
    Task<PagedResult<ProyectoDto>> ListarAsync(int pagina, int tamanoPagina, string? filtroNombre, CancellationToken ct = default);
    Task<ProyectoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<ProyectoDto> CrearAsync(CrearProyectoRequest request, CancellationToken ct = default);
    Task<ProyectoDto> ActualizarAsync(Guid id, ActualizarProyectoRequest request, CancellationToken ct = default);
    Task EliminarAsync(Guid id, CancellationToken ct = default);
}
