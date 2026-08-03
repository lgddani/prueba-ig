using System.Collections.Concurrent;

namespace Kanban.Infrastructure.Realtime;

/// <summary>
/// Registro en memoria de conexiones activas por tablero, usado únicamente para el
/// indicador (opcional) de usuarios conectados.
/// </summary>
public class ConnectedUsersTracker
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, string>> _porTablero = new();
    private readonly ConcurrentDictionary<string, Guid> _tableroPorConexion = new();

    public IReadOnlyList<string> Agregar(Guid proyectoId, string connectionId, string nombreUsuario)
    {
        var conexiones = _porTablero.GetOrAdd(proyectoId, _ => new ConcurrentDictionary<string, string>());
        conexiones[connectionId] = nombreUsuario;
        _tableroPorConexion[connectionId] = proyectoId;
        return conexiones.Values.Distinct().OrderBy(n => n).ToList();
    }

    public (Guid ProyectoId, IReadOnlyList<string> Nombres)? Quitar(string connectionId)
    {
        if (!_tableroPorConexion.TryRemove(connectionId, out var proyectoId))
            return null;

        if (_porTablero.TryGetValue(proyectoId, out var conexiones))
        {
            conexiones.TryRemove(connectionId, out _);
            return (proyectoId, conexiones.Values.Distinct().OrderBy(n => n).ToList());
        }

        return (proyectoId, Array.Empty<string>());
    }
}
