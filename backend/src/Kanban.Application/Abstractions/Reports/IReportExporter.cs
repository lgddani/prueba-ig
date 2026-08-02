using Kanban.Application.DTOs.Reportes;

namespace Kanban.Application.Abstractions.Reports;

public enum FormatoReporte
{
    Pdf,
    Excel
}

/// <summary>
/// Puerto de exportación
/// </summary>
public interface IReportExporter
{
    FormatoReporte Formato { get; }
    string ContentType { get; }
    string ExtensionArchivo { get; }
    Task<byte[]> ExportarAsync(ReporteProyectoDto reporte, CancellationToken ct = default);
}
