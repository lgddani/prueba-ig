namespace Kanban.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken ct = default);
}
