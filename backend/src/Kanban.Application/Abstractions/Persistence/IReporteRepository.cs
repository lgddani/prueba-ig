using Kanban.Application.DTOs.Reportes;

namespace Kanban.Application.Abstractions.Persistence;

/// <summary>
/// Puerto dedicado a la única consulta que alimenta ambos formatos de reporte
/// (PDF y Excel), evitando dos round-trips distintos a la base de datos.
/// </summary>
public interface IReporteRepository
{
    Task<ReporteProyectoDto?> ObtenerReporteProyectoAsync(Guid proyectoId, CancellationToken ct = default);
}
