using Kanban.Domain.Services;
using Xunit;

namespace Kanban.UnitTests.Domain;

public class OrdenTareaCalculatorTests
{
    [Fact]
    public void CalcularNuevaPosicion_ColumnaVacia_DevuelveGapInicial()
    {
        var resultado = OrdenTareaCalculator.CalcularNuevaPosicion(null, null);

        Assert.Equal(OrdenTareaCalculator.Gap, resultado);
    }

    [Fact]
    public void CalcularNuevaPosicion_EntreDosVecinas_DevuelvePromedio()
    {
        // Tarea A en 65536, Tarea B en 131072: insertar entre ambas.
        var resultado = OrdenTareaCalculator.CalcularNuevaPosicion(65536, 131072);

        Assert.Equal(98304, resultado);
        Assert.True(resultado > 65536 && resultado < 131072);
    }

    [Fact]
    public void CalcularNuevaPosicion_AlFinalDeLaColumna_SumaUnGap()
    {
        var resultado = OrdenTareaCalculator.CalcularNuevaPosicion(65536, null);

        Assert.Equal(65536 + OrdenTareaCalculator.Gap, resultado);
    }

    [Fact]
    public void CalcularNuevaPosicion_AlInicioDeLaColumna_DevuelveMitadDelPrimero()
    {
        var resultado = OrdenTareaCalculator.CalcularNuevaPosicion(null, 65536);

        Assert.Equal(32768, resultado);
    }

    [Fact]
    public void CalcularNuevaPosicion_SinEspacioEntreVecinas_DevuelveNullParaSenalizarRebalanceo()
    {
        // Vecinas contiguas (diferencia de 1): no hay hueco disponible.
        var resultado = OrdenTareaCalculator.CalcularNuevaPosicion(100, 101);

        Assert.Null(resultado);
    }

    [Fact]
    public void Rebalancear_AsignaOrdenesEquiespaciadosRespetandoElOrdenRecibido()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        var resultado = OrdenTareaCalculator.Rebalancear(new[] { id1, id2, id3 });

        Assert.Equal(new[] { id1, id2, id3 }, resultado.Select(r => r.TareaId));
        Assert.Equal(new[] { OrdenTareaCalculator.Gap, OrdenTareaCalculator.Gap * 2, OrdenTareaCalculator.Gap * 3 }, resultado.Select(r => r.NuevoOrden));
    }
}
