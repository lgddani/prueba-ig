using Kanban.Domain.Entities;
using Kanban.Domain.Enums;
using Xunit;

namespace Kanban.UnitTests.Domain;

public class ProyectoTests
{
    [Fact]
    public void Crear_ConFechaFinAnteriorAFechaInicio_LanzaArgumentException()
    {
        var inicio = new DateOnly(2026, 8, 1);
        var fin = new DateOnly(2026, 7, 1);

        Assert.Throws<ArgumentException>(() =>
            Proyecto.Crear("Proyecto X", "desc", inicio, fin, EstadoProyecto.Planificado));
    }

    [Fact]
    public void Crear_ConDatosValidos_AsignaPropiedadesCorrectamente()
    {
        var inicio = new DateOnly(2026, 8, 1);
        var fin = new DateOnly(2026, 9, 1);

        var proyecto = Proyecto.Crear("  Proyecto X  ", "desc", inicio, fin, EstadoProyecto.EnProgreso);

        Assert.Equal("Proyecto X", proyecto.Nombre);
        Assert.Equal(EstadoProyecto.EnProgreso, proyecto.Estado);
        Assert.Empty(proyecto.Columnas);
    }
}
