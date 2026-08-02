using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Abstractions.Security;
using Kanban.Application.DTOs.Auth;
using Kanban.Application.Exceptions;

namespace Kanban.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorCorreoAsync(request.Correo.Trim().ToLowerInvariant(), ct);
        if (usuario is null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash))
            throw new InvalidCredentialsException();

        var token = _jwtTokenGenerator.GenerarToken(usuario);

        return new LoginResponse(
            token.Token,
            token.ExpiraUtc,
            new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Correo));
    }
}
