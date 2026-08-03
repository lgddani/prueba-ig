using Kanban.Domain.Entities;
using Kanban.Domain.Enums;
using Xunit;

namespace Kanban.UnitTests.Domain;

public class ColumnaTests
{
    [Fact]
    public void PuedeEliminarse_SinTareas_DevuelveTrue()
    {
        var columna = Columna.Crear("Por hacer", 0, Guid.NewGuid());

        Assert.True(columna.PuedeEliminarse());
    }

    [Fact]
    public void PuedeEliminarse_ConTareas_DevuelveFalse()
    {
        var columna = Columna.Crear("Por hacer", 0, Guid.NewGuid());

        // La colección de tareas se puebla vía el FK inverso al cargar con EF Core;
        // aquí se simula el mismo efecto sobre la lista de respaldo para probar la
        // regla de negocio en aislamiento, sin depender de un DbContext real.
        var tareas = (List<Tarea>)typeof(Columna)
            .GetField("_tareas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(columna)!;
        tareas.Add(Tarea.Crear("Tarea 1", null, Prioridad.Media, null, columna.Id, 65536));

        Assert.False(columna.PuedeEliminarse());
    }
}
