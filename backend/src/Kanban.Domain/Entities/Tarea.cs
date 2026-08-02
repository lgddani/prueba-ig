using Kanban.Domain.Common;
using Kanban.Domain.Enums;

namespace Kanban.Domain.Entities;

public class Tarea : Entity
{
    public string Titulo { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public Prioridad Prioridad { get; private set; }
    public Guid? ResponsableId { get; private set; }
    public Guid ColumnaId { get; private set; }
    public long Orden { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Tarea() { }

    public static Tarea Crear(string titulo, string? descripcion, Prioridad prioridad, Guid? responsableId, Guid columnaId, long orden)
    {
        Validar(titulo, columnaId);

        return new Tarea
        {
            Titulo = titulo.Trim(),
            Descripcion = descripcion?.Trim(),
            Prioridad = prioridad,
            ResponsableId = responsableId,
            ColumnaId = columnaId,
            Orden = orden,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public void Actualizar(string titulo, string? descripcion, Prioridad prioridad, Guid? responsableId)
    {
        Validar(titulo, ColumnaId);
        Titulo = titulo.Trim();
        Descripcion = descripcion?.Trim();
        Prioridad = prioridad;
        ResponsableId = responsableId;
    }

    // traslada la tarea a otra columna (o la misma) y le asigna un nuevo orden
    public void MoverA(Guid columnaId, long nuevoOrden)
    {
        if (columnaId == Guid.Empty)
            throw new ArgumentException("La columna destino es obligatoria.", nameof(columnaId));

        ColumnaId = columnaId;
        Orden = nuevoOrden;
    }

    public void ReasignarOrden(long nuevoOrden) => Orden = nuevoOrden;

    private static void Validar(string titulo, Guid columnaId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título de la tarea es obligatorio.", nameof(titulo));
        if (columnaId == Guid.Empty)
            throw new ArgumentException("La tarea debe pertenecer a una columna.", nameof(columnaId));
    }
}
