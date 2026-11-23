namespace GoatJersey_API.Helpers;

/// <summary>Se rompe una regla de negocio o los datos no son válidos (400).</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Credenciales o token inválidos (401).</summary>
public class AppUnauthorizedException : Exception
{
    public AppUnauthorizedException(string message) : base(message) { }
}

/// <summary>El usuario no es dueño del recurso o no tiene el rol (403).</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

/// <summary>El recurso no existe (404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
