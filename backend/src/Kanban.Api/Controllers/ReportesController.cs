using Kanban.Application.Abstractions.Reports;
using Kanban.Application.Services.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/proyectos/{proyectoId:guid}/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService) => _reporteService = reporteService;

    [HttpGet("pdf")]
    public async Task<IActionResult> DescargarPdf(Guid proyectoId, CancellationToken ct)
        => await DescargarAsync(proyectoId, FormatoReporte.Pdf, ct);

    [HttpGet("excel")]
    public async Task<IActionResult> DescargarExcel(Guid proyectoId, CancellationToken ct)
        => await DescargarAsync(proyectoId, FormatoReporte.Excel, ct);

    private async Task<IActionResult> DescargarAsync(Guid proyectoId, FormatoReporte formato, CancellationToken ct)
    {
        var archivo = await _reporteService.GenerarAsync(proyectoId, formato, ct);
        return File(archivo.Contenido, archivo.ContentType, archivo.NombreArchivo);
    }
}
