using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kanban.Infrastructure.Realtime;

/// <summary>
/// Hub SignalR autenticado con el mismo JWT de sesión (ver configuración de
/// JwtBearerEvents.OnMessageReceived en Program.cs, que permite pasar el token por
/// query string "access_token" ya que los navegadores no permiten cabeceras
/// personalizadas en la negociación WebSocket)
/// </summary>
[Authorize]
public class BoardHub : Hub
{
    private readonly ConnectedUsersTracker _connectedUsers;

    public BoardHub(ConnectedUsersTracker connectedUsers) => _connectedUsers = connectedUsers;

    public static string GroupName(Guid proyectoId) => $"board:{proyectoId}";

    public async Task SuscribirseATablero(Guid proyectoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(proyectoId));

        var nombre = Context.User?.FindFirst("name")?.Value ?? "Usuario";
        var nombres = _connectedUsers.Agregar(proyectoId, Context.ConnectionId, nombre);
        await Clients.Group(GroupName(proyectoId)).SendAsync(BoardEvents.UsuariosConectados, nombres);
    }

    public async Task DesuscribirseDeTablero(Guid proyectoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(proyectoId));

        var resultado = _connectedUsers.Quitar(Context.ConnectionId);
        if (resultado is { } r)
            await Clients.Group(GroupName(r.ProyectoId)).SendAsync(BoardEvents.UsuariosConectados, r.Nombres);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var resultado = _connectedUsers.Quitar(Context.ConnectionId);
        if (resultado is { } r)
            await Clients.Group(GroupName(r.ProyectoId)).SendAsync(BoardEvents.UsuariosConectados, r.Nombres);

        await base.OnDisconnectedAsync(exception);
    }
}
