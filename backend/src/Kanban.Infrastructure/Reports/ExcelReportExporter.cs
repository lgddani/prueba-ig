using ClosedXML.Excel;
using Kanban.Application.Abstractions.Reports;
using Kanban.Application.DTOs.Reportes;

namespace Kanban.Infrastructure.Reports;

public class ExcelReportExporter : IReportExporter
{
    public FormatoReporte Formato => FormatoReporte.Excel;
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string ExtensionArchivo => "xlsx";

    public Task<byte[]> ExportarAsync(ReporteProyectoDto reporte, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Reporte");

        hoja.Cell(1, 1).Value = reporte.ProyectoNombre;
        hoja.Cell(1, 1).Style.Font.Bold = true;
        hoja.Cell(1, 1).Style.Font.FontSize = 16;
        hoja.Range(1, 1, 1, 4).Merge();

        hoja.Cell(2, 1).Value = reporte.ProyectoDescripcion ?? string.Empty;
        hoja.Range(2, 1, 2, 4).Merge();

        hoja.Cell(3, 1).Value = $"Periodo: {reporte.FechaInicio:yyyy-MM-dd} a {reporte.FechaFinPrevista:yyyy-MM-dd}   ·   Estado: {reporte.Estado}";
        hoja.Range(3, 1, 3, 4).Merge();

        hoja.Cell(4, 1).Value = $"Generado: {reporte.FechaGeneracion:yyyy-MM-dd HH:mm} UTC";
        hoja.Cell(4, 1).Style.Font.FontColor = XLColor.Gray;
        hoja.Range(4, 1, 4, 4).Merge();

        const int filaEncabezado = 6;
        var encabezados = new[] { "Tarea", "Columna", "Responsable", "Prioridad" };
        for (var i = 0; i < encabezados.Length; i++)
            hoja.Cell(filaEncabezado, i + 1).Value = encabezados[i];

        var rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, encabezados.Length);
        rangoEncabezado.Style.Font.Bold = true;
        rangoEncabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

        var fila = filaEncabezado + 1;
        foreach (var tarea in reporte.Tareas)
        {
            hoja.Cell(fila, 1).Value = tarea.Titulo;
            hoja.Cell(fila, 2).Value = tarea.Columna;
            hoja.Cell(fila, 3).Value = tarea.Responsable ?? "Sin asignar";
            hoja.Cell(fila, 4).Value = tarea.Prioridad;
            fila++;
        }

        hoja.Columns(1, 4).AdjustToContents();
        hoja.Column(1).Width = Math.Max(hoja.Column(1).Width, 30);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
