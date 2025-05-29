using System;
using System.Linq;
using System.Threading.Tasks;
using Capylender.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Capylender.API.Services;

public class PesquisaSatisfacaoJob
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public PesquisaSatisfacaoJob(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task EnviarPesquisasAsync()
    {
        var agora = DateTime.UtcNow;
        var umaHoraAtras = agora.AddHours(-1);
        var agendamentos = await _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Servico)
            .Where(a => a.DataHoraFim <= agora && a.DataHoraFim > umaHoraAtras && a.Confirmado && !a.Cancelado)
            .ToListAsync();

        foreach (var ag in agendamentos)
        {
            // Link fictício para feedback (pode ser ajustado para um endpoint real)
            var link = $"https://seusite.com/feedback?agendamentoId={ag.Id}";
            await _emailService.EnviarEmailAsync(
                ag.Cliente.Email,
                "Como foi seu atendimento?",
                $"<p>Olá, {ag.Cliente.Nome}!</p><p>Seu agendamento para o serviço <b>{ag.Servico.Nome}</b> foi concluído. Por favor, <a href='{link}'>clique aqui</a> para avaliar o atendimento.</p>");
        }
    }
} 