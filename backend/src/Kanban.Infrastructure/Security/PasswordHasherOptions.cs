namespace Kanban.Infrastructure.Security;

/// <summary>
/// Pepper: secreto único del servidor (no almacenado por usuario, a diferencia del
/// salt) que se concatena a la contraseña antes de derivar la clave
/// </summary>
public class PasswordHasherOptions
{
    public string Pepper { get; set; } = string.Empty;
}
