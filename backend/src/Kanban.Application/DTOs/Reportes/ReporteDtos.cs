namespace Kanban.Application.DTOs.Reportes;

/// <summary>
/// Única estructura de transferencia que alimenta tanto al exportador PDF como al
/// Excel
/// </summary>
public record ReporteProyectoDto(
    string ProyectoNombre,
    string? ProyectoDescripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinPrevista,
    string Estado,
    DateTime FechaGeneracion,
    IReadOnlyList<ReporteTareaDto> Tareas
);

public record ReporteTareaDto(
    string Titulo,
    string Columna,
    string? Responsable,
    string Prioridad
);
