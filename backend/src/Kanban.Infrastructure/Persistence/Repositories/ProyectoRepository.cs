using Kanban.Application.Abstractions.Persistence;
using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class ProyectoRepository : IProyectoRepository
{
    private readonly KanbanDbContext _context;

    public ProyectoRepository(KanbanDbContext context) => _context = context;

    public Task<Proyecto?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => _context.Proyectos.Include(p => p.Columnas).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(IReadOnlyList<Proyecto> Items, int Total)> ListarPaginadoAsync(
        int pagina, int tamanoPagina, string? filtroNombre, CancellationToken ct = default)
    {
        var query = _context.Proyectos.Include(p => p.Columnas).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
            query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{filtroNombre}%"));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.FechaCreacion)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AgregarAsync(Proyecto proyecto, CancellationToken ct = default)
        => await _context.Proyectos.AddAsync(proyecto, ct);

    public void Eliminar(Proyecto proyecto) => _context.Proyectos.Remove(proyecto);
}
