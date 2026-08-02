using Kanban.Application.Abstractions.Reports;

namespace Kanban.Application.Services.Reportes;

public record ArchivoGenerado(byte[] Contenido, string ContentType, string NombreArchivo);

public interface IReporteService
{
    Task<ArchivoGenerado> GenerarAsync(Guid proyectoId, FormatoReporte formato, CancellationToken ct = default);
}
