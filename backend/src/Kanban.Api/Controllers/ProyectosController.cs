using Kanban.Application.Common;
using Kanban.Application.DTOs.Proyectos;
using Kanban.Application.Services.Proyectos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/proyectos")]
public class ProyectosController : ControllerBase
{
    private readonly IProyectoService _proyectoService;

    public ProyectosController(IProyectoService proyectoService) => _proyectoService = proyectoService;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProyectoDto>>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        [FromQuery] string? nombre = null,
        CancellationToken ct = default)
        => Ok(await _proyectoService.ListarAsync(pagina, tamanoPagina, nombre, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProyectoDto>> ObtenerPorId(Guid id, CancellationToken ct)
        => Ok(await _proyectoService.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProyectoDto>> Crear(CrearProyectoRequest request, CancellationToken ct)
    {
        var proyecto = await _proyectoService.CrearAsync(request, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = proyecto.Id }, proyecto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProyectoDto>> Actualizar(Guid id, ActualizarProyectoRequest request, CancellationToken ct)
        => Ok(await _proyectoService.ActualizarAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _proyectoService.EliminarAsync(id, ct);
        return NoContent();
    }
}
