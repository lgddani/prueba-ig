using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Abstractions.Realtime;
using Kanban.Application.Abstractions.Reports;
using Kanban.Application.Abstractions.Security;
using Kanban.Infrastructure.Persistence;
using Kanban.Infrastructure.Persistence.Repositories;
using Kanban.Infrastructure.Realtime;
using Kanban.Infrastructure.Reports;
using Kanban.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Kanban.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection'.");

        services.AddDbContext<KanbanDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<PasswordHasherOptions>(configuration.GetSection("PasswordHasher"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProyectoRepository, ProyectoRepository>();
        services.AddScoped<IColumnaRepository, ColumnaRepository>();
        services.AddScoped<ITareaRepository, TareaRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IBoardNotifier, SignalRBoardNotifier>();
        services.AddSingleton<ConnectedUsersTracker>();

        services.AddScoped<IReportExporter, PdfReportExporter>();
        services.AddScoped<IReportExporter, ExcelReportExporter>();
        services.AddScoped<ReportExporterFactory>();

        services.AddSignalR().AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
