using Kanban.Domain.Common;
using Kanban.Domain.Enums;

namespace Kanban.Domain.Entities;

public class Proyecto : Entity
{
    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFinPrevista { get; private set; }
    public EstadoProyecto Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private readonly List<Columna> _columnas = new();
    public IReadOnlyCollection<Columna> Columnas => _columnas.AsReadOnly();

    private Proyecto() { }

    public static Proyecto Crear(string nombre, string? descripcion, DateOnly fechaInicio, DateOnly fechaFinPrevista, EstadoProyecto estado)
    {
        Validar(nombre, fechaInicio, fechaFinPrevista);

        return new Proyecto
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            FechaInicio = fechaInicio,
            FechaFinPrevista = fechaFinPrevista,
            Estado = estado,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public void Actualizar(string nombre, string? descripcion, DateOnly fechaInicio, DateOnly fechaFinPrevista, EstadoProyecto estado)
    {
        Validar(nombre, fechaInicio, fechaFinPrevista);

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        FechaInicio = fechaInicio;
        FechaFinPrevista = fechaFinPrevista;
        Estado = estado;
    }

    private static void Validar(string nombre, DateOnly fechaInicio, DateOnly fechaFinPrevista)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del proyecto es obligatorio.", nameof(nombre));
        if (fechaFinPrevista < fechaInicio)
            throw new ArgumentException("La fecha de fin prevista no puede ser anterior a la fecha de inicio.");
    }
}
