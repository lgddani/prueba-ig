namespace Kanban.Application.Abstractions.Reports;

/// <summary>
/// Selecciona el exportador adecuado (patrón Strategy resuelto vía Factory + inyección
/// de dependencias)
/// </summary>
public class ReportExporterFactory
{
    private readonly IReadOnlyDictionary<FormatoReporte, IReportExporter> _exporters;

    public ReportExporterFactory(IEnumerable<IReportExporter> exporters)
    {
        _exporters = exporters.ToDictionary(e => e.Formato);
    }

    public IReportExporter Obtener(FormatoReporte formato)
    {
        if (!_exporters.TryGetValue(formato, out var exporter))
            throw new NotSupportedException($"No hay un exportador registrado para el formato '{formato}'.");
        return exporter;
    }
}
