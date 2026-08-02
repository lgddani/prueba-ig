namespace Kanban.Domain.Services;

/// Estrategia de posicionamiento por huecos:

public static class OrdenTareaCalculator
{
    public const long Gap = 65536;

    public static long CalcularOrdenAlFinal(int cantidadTareasExistentes)
        => (cantidadTareasExistentes + 1) * Gap;

    // Calcula la nueva posición de una tarea dado el orden de la tarea inmediatamente
    // anterior y siguiente en la columna destino
    public static long? CalcularNuevaPosicion(long? ordenAnterior, long? ordenSiguiente)
    {
        if (ordenAnterior is null && ordenSiguiente is null)
            return Gap;

        if (ordenAnterior is null)
        {
            var siguiente = ordenSiguiente!.Value;
            return siguiente > 1 ? siguiente / 2 : null;
        }

        if (ordenSiguiente is null)
            return ordenAnterior.Value + Gap;

        var anterior = ordenAnterior.Value;
        var diferencia = ordenSiguiente.Value - anterior;

        return diferencia > 1 ? anterior + diferencia / 2 : null;
    }

    /// <summary>
    /// Reasigna órdened
    /// </summary>
    public static IReadOnlyList<(Guid TareaId, long NuevoOrden)> Rebalancear(IEnumerable<Guid> tareaIdsEnOrden)
        => tareaIdsEnOrden
            .Select((id, index) => (TareaId: id, NuevoOrden: (index + 1) * Gap))
            .ToList();
}
