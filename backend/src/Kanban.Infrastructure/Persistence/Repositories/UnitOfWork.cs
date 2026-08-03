using Kanban.Application.Abstractions.Persistence;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly KanbanDbContext _context;

    public UnitOfWork(KanbanDbContext context) => _context = context;

    public Task<int> GuardarCambiosAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
