namespace Kanban.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int PaginaActual { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas => TamanoPagina == 0 ? 0 : (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}
