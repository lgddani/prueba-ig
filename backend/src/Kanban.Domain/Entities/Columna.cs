using Kanban.Domain.Common;

namespace Kanban.Domain.Entities;

public class Columna : Entity
{
    public string Nombre { get; private set; } = default!;
    public int Orden { get; private set; }
    public Guid ProyectoId { get; private set; }

    private readonly List<Tarea> _tareas = new();
    public IReadOnlyCollection<Tarea> Tareas => _tareas.AsReadOnly();

    private Columna() { }

    public static Columna Crear(string nombre, int orden, Guid proyectoId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la columna es obligatorio.", nameof(nombre));
        if (proyectoId == Guid.Empty)
            throw new ArgumentException("La columna debe pertenecer a un proyecto.", nameof(proyectoId));

        return new Columna
        {
            Nombre = nombre.Trim(),
            Orden = orden,
            ProyectoId = proyectoId
        };
    }

    public void Renombrar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la columna es obligatorio.", nameof(nombre));
        Nombre = nombre.Trim();
    }

    public void CambiarOrden(int nuevoOrden) => Orden = nuevoOrden;

    /// no se permite eliminar una columna que contenga tareas
    public bool PuedeEliminarse() => _tareas.Count == 0;
}
