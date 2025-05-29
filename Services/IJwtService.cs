namespace Capylender.API.Services;

public interface IJwtService
{
    string GerarToken(int usuarioId, string nome, string email, string role);
} 