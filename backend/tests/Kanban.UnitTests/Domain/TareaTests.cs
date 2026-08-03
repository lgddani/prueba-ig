using Kanban.Domain.Entities;
using Kanban.Domain.Enums;
using Xunit;

namespace Kanban.UnitTests.Domain;

public class TareaTests
{
    [Fact]
    public void MoverA_CambiaColumnaYOrden()
    {
        var columnaOrigen = Guid.NewGuid();
        var columnaDestino = Guid.NewGuid();
        var tarea = Tarea.Crear("Tarea 1", null, Prioridad.Baja, null, columnaOrigen, 65536);

        tarea.MoverA(columnaDestino, 32768);

        Assert.Equal(columnaDestino, tarea.ColumnaId);
        Assert.Equal(32768, tarea.Orden);
    }

    [Fact]
    public void Crear_ConTituloVacio_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            Tarea.Crear("   ", null, Prioridad.Baja, null, Guid.NewGuid(), 65536));
    }
}
