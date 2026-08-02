using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Abstractions.Reports;
using Kanban.Application.Exceptions;

namespace Kanban.Application.Services.Reportes;

public class ReporteService : IReporteService
{
    private readonly IReporteRepository _reporteRepository;
    private readonly ReportExporterFactory _exporterFactory;

    public ReporteService(IReporteRepository reporteRepository, ReportExporterFactory exporterFactory)
    {
        _reporteRepository = reporteRepository;
        _exporterFactory = exporterFactory;
    }

    public async Task<ArchivoGenerado> GenerarAsync(Guid proyectoId, FormatoReporte formato, CancellationToken ct = default)
    {
        var reporte = await _reporteRepository.ObtenerReporteProyectoAsync(proyectoId, ct)
            ?? throw new NotFoundException("Proyecto", proyectoId);

        var exporter = _exporterFactory.Obtener(formato);
        var contenido = await exporter.ExportarAsync(reporte, ct);

        var nombreBase = string.Join('_', reporte.ProyectoNombre.Split(Path.GetInvalidFileNameChars()));
        var nombreArchivo = $"reporte_{nombreBase}_{DateTime.UtcNow:yyyyMMddHHmm}.{exporter.ExtensionArchivo}";

        return new ArchivoGenerado(contenido, exporter.ContentType, nombreArchivo);
    }
}
