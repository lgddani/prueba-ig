using Kanban.Application.Abstractions.Persistence;
using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class ColumnaRepository : IColumnaRepository
{
    private readonly KanbanDbContext _context;

    public ColumnaRepository(KanbanDbContext context) => _context = context;

    public Task<Columna?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => _context.Columnas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Columna?> ObtenerConTareasAsync(Guid id, CancellationToken ct = default)
        => _context.Columnas.Include(c => c.Tareas).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Columna>> ListarPorProyectoAsync(Guid proyectoId, CancellationToken ct = default)
        => await _context.Columnas.Include(c => c.Tareas)
            .Where(c => c.ProyectoId == proyectoId)
            .OrderBy(c => c.Orden)
            .ToListAsync(ct);

    public async Task AgregarAsync(Columna columna, CancellationToken ct = default)
        => await _context.Columnas.AddAsync(columna, ct);

    public void Eliminar(Columna columna) => _context.Columnas.Remove(columna);
}
