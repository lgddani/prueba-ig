namespace Kanban.Application.Exceptions;

/// <summary>El recurso solicitado no existe.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entidad, object id) : base($"{entidad} '{id}' no fue encontrado.") { }
}

/// <summary>Violación de una regla de negocio (mapea a HTTP 409 Conflict).</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Credenciales inválidas al autenticar (mapea a HTTP 401).</summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Correo o contraseña incorrectos.") { }
}
