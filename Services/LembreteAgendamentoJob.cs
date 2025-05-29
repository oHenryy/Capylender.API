using System;
using System.Linq;
using System.Threading.Tasks;
using Capylender.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Capylender.API.Services;

public class LembreteAgendamentoJob
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public LembreteAgendamentoJob(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task EnviarLembretesAsync()
    {
        var agora = DateTime.UtcNow;
        var daquiDuasHoras = agora.AddHours(2);
        var agendamentos = await _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .Where(a => a.DataHoraInicio > agora && a.DataHoraInicio <= daquiDuasHoras && !a.Cancelado && a.Confirmado)
            .ToListAsync();

        foreach (var ag in agendamentos)
        {
            // Lembrete para cliente
            await _emailService.EnviarEmailAsync(
                ag.Cliente.Email,
                "Lembrete de agendamento",
                $"<p>Olá, {ag.Cliente.Nome}!</p><p>Lembrete: seu agendamento para o serviço <b>{ag.Servico.Nome}</b> com o profissional <b>{ag.Profissional.Nome}</b> está marcado para <b>{ag.DataHoraInicio:dd/MM/yyyy HH:mm}</b>.</p>");
            // Lembrete para profissional
            await _emailService.EnviarEmailAsync(
                ag.Profissional.Email,
                "Lembrete de agendamento",
                $"<p>Olá, {ag.Profissional.Nome}!</p><p>Lembrete: você tem um agendamento para o serviço <b>{ag.Servico.Nome}</b> com o cliente <b>{ag.Cliente.Nome}</b> em <b>{ag.DataHoraInicio:dd/MM/yyyy HH:mm}</b>.</p>");
        }
    }
} 