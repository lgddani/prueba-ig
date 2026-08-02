using System.ComponentModel.DataAnnotations;
using Kanban.Domain.Enums;

namespace Kanban.Application.DTOs.Proyectos;

public record ProyectoDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinPrevista,
    EstadoProyecto Estado,
    DateTime FechaCreacion,
    int TotalColumnas
);

public record CrearProyectoRequest(
    [Required, MaxLength(150)] string Nombre,
    [MaxLength(2000)] string? Descripcion,
    [Required] DateOnly FechaInicio,
    [Required] DateOnly FechaFinPrevista,
    EstadoProyecto Estado
);

public record ActualizarProyectoRequest(
    [Required, MaxLength(150)] string Nombre,
    [MaxLength(2000)] string? Descripcion,
    [Required] DateOnly FechaInicio,
    [Required] DateOnly FechaFinPrevista,
    EstadoProyecto Estado
);
