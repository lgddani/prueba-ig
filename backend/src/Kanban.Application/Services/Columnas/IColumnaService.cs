using Kanban.Application.DTOs.Columnas;

namespace Kanban.Application.Services.Columnas;

public interface IColumnaService
{
    Task<IReadOnlyList<ColumnaDto>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default);
    Task<ColumnaDto> CrearAsync(Guid proyectoId, CrearColumnaRequest request, CancellationToken ct = default);
    Task<ColumnaDto> ActualizarAsync(Guid proyectoId, Guid columnaId, ActualizarColumnaRequest request, CancellationToken ct = default);
    Task EliminarAsync(Guid proyectoId, Guid columnaId, CancellationToken ct = default);
    Task ReordenarAsync(Guid proyectoId, ReordenarColumnasRequest request, CancellationToken ct = default);
}
