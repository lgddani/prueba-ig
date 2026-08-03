using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.DTOs.Reportes;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly KanbanDbContext _context;

    public ReporteRepository(KanbanDbContext context) => _context = context;

    public async Task<ReporteProyectoDto?> ObtenerReporteProyectoAsync(Guid proyectoId, CancellationToken ct = default)
    {
        var fechaGeneracion = DateTime.UtcNow;

        return await _context.Proyectos
            .AsNoTracking()
            .Where(p => p.Id == proyectoId)
            .Select(p => new ReporteProyectoDto(
                p.Nombre,
                p.Descripcion,
                p.FechaInicio,
                p.FechaFinPrevista,
                p.Estado.ToString(),
                fechaGeneracion,
                p.Columnas
                    .OrderBy(c => c.Orden)
                    .SelectMany(c => c.Tareas.OrderBy(t => t.Orden).Select(t => new ReporteTareaDto(
                        t.Titulo,
                        c.Nombre,
                        t.ResponsableId == null
                            ? null
                            : _context.Usuarios.Where(u => u.Id == t.ResponsableId).Select(u => u.Nombre).FirstOrDefault(),
                        t.Prioridad.ToString())))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
    }
}
