using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Persistence;

public interface IProyectoRepository
{
    Task<Proyecto?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Proyecto> Items, int Total)> ListarPaginadoAsync(
        int pagina, int tamanoPagina, string? filtroNombre, CancellationToken ct = default);
    Task AgregarAsync(Proyecto proyecto, CancellationToken ct = default);
    void Eliminar(Proyecto proyecto);
}
