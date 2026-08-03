using Kanban.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kanban.UnitTests.Infrastructure;

public class PasswordHasherTests
{
    private static PasswordHasher CrearHasher(string pepper = "pepper-de-prueba")
        => new(Options.Create(new PasswordHasherOptions { Pepper = pepper }));

    [Fact]
    public void Hash_LuegoVerify_ConLaMismaContrasena_DevuelveTrue()
    {
        var hasher = CrearHasher();
        var hash = hasher.Hash("Kanban#2026");

        Assert.True(hasher.Verify("Kanban#2026", hash));
    }

    [Fact]
    public void Verify_ConContrasenaIncorrecta_DevuelveFalse()
    {
        var hasher = CrearHasher();
        var hash = hasher.Hash("Kanban#2026");

        Assert.False(hasher.Verify("otra-contrasena", hash));
    }

    [Fact]
    public void Verify_ConPepperDistintoAlUsadoParaElHash_DevuelveFalse()
    {
        var hash = CrearHasher("pepper-original").Hash("Kanban#2026");

        Assert.False(CrearHasher("pepper-diferente").Verify("Kanban#2026", hash));
    }
}
