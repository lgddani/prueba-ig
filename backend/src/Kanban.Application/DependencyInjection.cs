using Kanban.Application.Services.Auth;
using Kanban.Application.Services.Columnas;
using Kanban.Application.Services.Proyectos;
using Kanban.Application.Services.Reportes;
using Kanban.Application.Services.Tareas;
using Kanban.Application.Services.Usuarios;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProyectoService, ProyectoService>();
        services.AddScoped<IColumnaService, ColumnaService>();
        services.AddScoped<ITareaService, TareaService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        return services;
    }
}
