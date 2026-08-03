using Kanban.Application.DTOs.Columnas;
using Kanban.Application.Services.Columnas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/proyectos/{proyectoId:guid}/columnas")]
public class ColumnasController : ControllerBase
{
    private readonly IColumnaService _columnaService;

    public ColumnasController(IColumnaService columnaService) => _columnaService = columnaService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ColumnaDto>>> Listar(Guid proyectoId, CancellationToken ct)
        => Ok(await _columnaService.ListarPorProyectoAsync(proyectoId, ct));

    [HttpPost]
    public async Task<ActionResult<ColumnaDto>> Crear(Guid proyectoId, CrearColumnaRequest request, CancellationToken ct)
        => Ok(await _columnaService.CrearAsync(proyectoId, request, ct));

    [HttpPut("{columnaId:guid}")]
    public async Task<ActionResult<ColumnaDto>> Actualizar(Guid proyectoId, Guid columnaId, ActualizarColumnaRequest request, CancellationToken ct)
        => Ok(await _columnaService.ActualizarAsync(proyectoId, columnaId, request, ct));

    [HttpDelete("{columnaId:guid}")]
    public async Task<IActionResult> Eliminar(Guid proyectoId, Guid columnaId, CancellationToken ct)
    {
        await _columnaService.EliminarAsync(proyectoId, columnaId, ct);
        return NoContent();
    }

    [HttpPut("reordenar")]
    public async Task<IActionResult> Reordenar(Guid proyectoId, ReordenarColumnasRequest request, CancellationToken ct)
    {
        await _columnaService.ReordenarAsync(proyectoId, request, ct);
        return NoContent();
    }
}
