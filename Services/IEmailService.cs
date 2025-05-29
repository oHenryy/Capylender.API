namespace Capylender.API.Services;

public interface IEmailService
{
    Task EnviarEmailAsync(string destinatario, string assunto, string mensagemHtml);
} 