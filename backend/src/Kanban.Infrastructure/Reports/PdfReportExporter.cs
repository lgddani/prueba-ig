using Kanban.Application.Abstractions.Reports;
using Kanban.Application.DTOs.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Kanban.Infrastructure.Reports;

public class PdfReportExporter : IReportExporter
{
    public FormatoReporte Formato => FormatoReporte.Pdf;
    public string ContentType => "application/pdf";
    public string ExtensionArchivo => "pdf";

    public Task<byte[]> ExportarAsync(ReporteProyectoDto reporte, CancellationToken ct = default)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text(reporte.ProyectoNombre).FontSize(20).Bold();

                    if (!string.IsNullOrWhiteSpace(reporte.ProyectoDescripcion))
                        col.Item().PaddingTop(2).Text(reporte.ProyectoDescripcion);

                    col.Item().PaddingTop(4).Text(
                        $"Periodo: {reporte.FechaInicio:yyyy-MM-dd} a {reporte.FechaFinPrevista:yyyy-MM-dd}   ·   Estado: {reporte.Estado}");

                    col.Item().PaddingTop(2).Text($"Generado: {reporte.FechaGeneracion:yyyy-MM-dd HH:mm} UTC")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);

                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CeldaEncabezado).Text("Tarea");
                        header.Cell().Element(CeldaEncabezado).Text("Columna");
                        header.Cell().Element(CeldaEncabezado).Text("Responsable");
                        header.Cell().Element(CeldaEncabezado).Text("Prioridad");
                    });

                    foreach (var tarea in reporte.Tareas)
                    {
                        table.Cell().Element(Celda).Text(tarea.Titulo);
                        table.Cell().Element(Celda).Text(tarea.Columna);
                        table.Cell().Element(Celda).Text(tarea.Responsable ?? "Sin asignar");
                        table.Cell().Element(Celda).Text(tarea.Prioridad);
                    }

                    if (reporte.Tareas.Count == 0)
                    {
                        table.Cell().ColumnSpan(4).Element(Celda).Text("Sin tareas registradas.").Italic();
                    }
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        });

        return Task.FromResult(documento.GeneratePdf());
    }

    private static IContainer CeldaEncabezado(IContainer container) => container
        .Background(Colors.Grey.Lighten3)
        .Padding(5)
        .BorderBottom(1)
        .BorderColor(Colors.Grey.Darken1)
        .DefaultTextStyle(x => x.SemiBold());

    private static IContainer Celda(IContainer container) => container
        .Padding(5)
        .BorderBottom(1)
        .BorderColor(Colors.Grey.Lighten2);
}
