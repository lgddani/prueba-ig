using System.ComponentModel.DataAnnotations;

namespace Kanban.Application.DTOs.Columnas;

public record ColumnaDto(Guid Id, string Nombre, int Orden, Guid ProyectoId, int TotalTareas);

public record CrearColumnaRequest([Required, MaxLength(100)] string Nombre);

public record ActualizarColumnaRequest([Required, MaxLength(100)] string Nombre);

public record ReordenarColumnasRequest([Required] IReadOnlyList<Guid> ColumnaIdsEnOrden);
