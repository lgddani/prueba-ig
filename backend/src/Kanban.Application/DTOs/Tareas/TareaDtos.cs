using System.ComponentModel.DataAnnotations;
using Kanban.Domain.Enums;

namespace Kanban.Application.DTOs.Tareas;

public record TareaDto(
    Guid Id,
    string Titulo,
    string? Descripcion,
    Prioridad Prioridad,
    Guid? ResponsableId,
    string? ResponsableNombre,
    Guid ColumnaId,
    long Orden,
    DateTime FechaCreacion
);

public record CrearTareaRequest(
    [Required, MaxLength(200)] string Titulo,
    [MaxLength(2000)] string? Descripcion,
    Prioridad Prioridad,
    Guid? ResponsableId,
    [Required] Guid ColumnaId
);

public record ActualizarTareaRequest(
    [Required, MaxLength(200)] string Titulo,
    [MaxLength(2000)] string? Descripcion,
    Prioridad Prioridad,
    Guid? ResponsableId
);

/// <summary>
/// PosicionDestino es el índice (0-based) deseado dentro de la columna destino tras el
/// arrastre
/// </summary>
public record MoverTareaRequest(
    [Required] Guid ColumnaDestinoId,
    [Required] int PosicionDestino
);
