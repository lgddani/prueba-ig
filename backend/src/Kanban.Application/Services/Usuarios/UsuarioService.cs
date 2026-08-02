using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.DTOs.Auth;

namespace Kanban.Application.Services.Usuarios;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarios;

    public UsuarioService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(CancellationToken ct = default)
    {
        var usuarios = await _usuarios.ListarAsync(ct);
        return usuarios
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Correo))
            .ToList();
    }
}
