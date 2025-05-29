using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Capylender.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _host;
    private readonly int _port;
    private readonly string _user;
    private readonly string _pass;
    private readonly string _from;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _host = _configuration["SmtpSettings:Host"];
        _port = int.Parse(_configuration["SmtpSettings:Port"]);
        _user = _configuration["SmtpSettings:User"];
        _pass = _configuration["SmtpSettings:Pass"];
        _from = _configuration["SmtpSettings:From"];
    }

    public async Task EnviarEmailAsync(string destinatario, string assunto, string mensagemHtml)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(_from));
        mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = assunto;
        mensagem.Body = new TextPart("html") { Text = mensagemHtml };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_user, _pass);
        await smtp.SendAsync(mensagem);
        await smtp.DisconnectAsync(true);
    }
} 