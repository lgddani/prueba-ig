using Kanban.Application.Abstractions.Persistence;
using Kanban.Application.Abstractions.Security;
using Kanban.Application.DTOs.Auth;
using Kanban.Application.Exceptions;
using Kanban.Application.Services.Auth;
using Kanban.Domain.Entities;
using Moq;
using Xunit;

namespace Kanban.UnitTests.Application;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ConCredencialesValidas_DevuelveTokenYUsuario()
    {
        var usuario = Usuario.Crear("Ana Torres", "ana@kanban.dev", "hash-almacenado");

        var usuarios = new Mock<IUsuarioRepository>();
        usuarios.Setup(r => r.ObtenerPorCorreoAsync("ana@kanban.dev", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("Kanban#2026", "hash-almacenado")).Returns(true);

        var jwt = new Mock<IJwtTokenGenerator>();
        jwt.Setup(j => j.GenerarToken(usuario)).Returns(new TokenGenerado("token-jwt", DateTime.UtcNow.AddHours(1)));

        var service = new AuthService(usuarios.Object, hasher.Object, jwt.Object);

        var respuesta = await service.LoginAsync(new LoginRequest("ana@kanban.dev", "Kanban#2026"));

        Assert.Equal("token-jwt", respuesta.Token);
        Assert.Equal(usuario.Id, respuesta.Usuario.Id);
    }

    [Fact]
    public async Task LoginAsync_ConContrasenaIncorrecta_LanzaInvalidCredentialsException()
    {
        var usuario = Usuario.Crear("Ana Torres", "ana@kanban.dev", "hash-almacenado");

        var usuarios = new Mock<IUsuarioRepository>();
        usuarios.Setup(r => r.ObtenerPorCorreoAsync("ana@kanban.dev", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var service = new AuthService(usuarios.Object, hasher.Object, Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => service.LoginAsync(new LoginRequest("ana@kanban.dev", "incorrecta")));
    }

    [Fact]
    public async Task LoginAsync_ConCorreoInexistente_LanzaInvalidCredentialsException()
    {
        var usuarios = new Mock<IUsuarioRepository>();
        usuarios.Setup(r => r.ObtenerPorCorreoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = new AuthService(usuarios.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => service.LoginAsync(new LoginRequest("nadie@kanban.dev", "cualquiera")));
    }
}
