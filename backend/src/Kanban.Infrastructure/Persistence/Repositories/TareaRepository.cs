using Kanban.Application.Abstractions.Persistence;
using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly KanbanDbContext _context;

    public TareaRepository(KanbanDbContext context) => _context = context;

    public Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => _context.Tareas.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Tarea>> ListarPorColumnaOrdenadoAsync(Guid columnaId, CancellationToken ct = default)
        => await _context.Tareas.Where(t => t.ColumnaId == columnaId).OrderBy(t => t.Orden).ToListAsync(ct);

    public async Task<IReadOnlyList<Tarea>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default)
        => await _context.Tareas
            .Where(t => _context.Columnas.Any(c => c.Id == t.ColumnaId && c.ProyectoId == proyectoId))
            .OrderBy(t => t.Orden)
            .ToListAsync(ct);

    public async Task AgregarAsync(Tarea tarea, CancellationToken ct = default)
        => await _context.Tareas.AddAsync(tarea, ct);

    public void Eliminar(Tarea tarea) => _context.Tareas.Remove(tarea);
}
