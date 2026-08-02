using Kanban.Domain.Common;

namespace Kanban.Domain.Entities;

public class Usuario : Entity
{
    public string Nombre { get; private set; } = default!;
    public string Correo { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public DateTime FechaCreacion { get; private set; }

    private Usuario() { }

    public static Usuario Crear(string nombre, string correo, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(correo))
            throw new ArgumentException("El correo es obligatorio.", nameof(correo));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(passwordHash));

        return new Usuario
        {
            Nombre = nombre.Trim(),
            Correo = correo.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FechaCreacion = DateTime.UtcNow
        };
    }
}
