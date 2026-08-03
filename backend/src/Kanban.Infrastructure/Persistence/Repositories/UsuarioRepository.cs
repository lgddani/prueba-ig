using Kanban.Application.Abstractions.Persistence;
using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly KanbanDbContext _context;

    public UsuarioRepository(KanbanDbContext context) => _context = context;

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default)
        => _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo, ct);

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken ct = default)
        => await _context.Usuarios.AsNoTracking().ToListAsync(ct);
}
