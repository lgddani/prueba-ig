using Kanban.Application.DTOs.Tareas;
using Kanban.Application.Services.Tareas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/proyectos/{proyectoId:guid}/tareas")]
public class TareasController : ControllerBase
{
    private readonly ITareaService _tareaService;

    public TareasController(ITareaService tareaService) => _tareaService = tareaService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TareaDto>>> Listar(Guid proyectoId, CancellationToken ct)
        => Ok(await _tareaService.ListarPorProyectoAsync(proyectoId, ct));

    [HttpPost]
    public async Task<ActionResult<TareaDto>> Crear(Guid proyectoId, CrearTareaRequest request, CancellationToken ct)
        => Ok(await _tareaService.CrearAsync(proyectoId, request, ct));

    [HttpPut("{tareaId:guid}")]
    public async Task<ActionResult<TareaDto>> Actualizar(Guid proyectoId, Guid tareaId, ActualizarTareaRequest request, CancellationToken ct)
        => Ok(await _tareaService.ActualizarAsync(proyectoId, tareaId, request, ct));

    [HttpDelete("{tareaId:guid}")]
    public async Task<IActionResult> Eliminar(Guid proyectoId, Guid tareaId, CancellationToken ct)
    {
        await _tareaService.EliminarAsync(proyectoId, tareaId, ct);
        return NoContent();
    }

    [HttpPost("{tareaId:guid}/mover")]
    public async Task<ActionResult<TareaDto>> Mover(Guid proyectoId, Guid tareaId, MoverTareaRequest request, CancellationToken ct)
        => Ok(await _tareaService.MoverAsync(proyectoId, tareaId, request, ct));
}
