using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Capylender.API.Services;

public class AgendamentoService : GenericService<Agendamento, AgendamentoDto, AgendamentoCreateUpdateDto>
{
    private readonly IGenericRepository<Agendamento> _agendamentoRepository;
    private readonly IGenericRepository<Disponibilidade> _disponibilidadeRepository;
    private readonly IGenericRepository<Servico> _servicoRepository;
    private readonly IGenericRepository<Profissional> _profissionalRepository;
    private readonly IGenericRepository<Cliente> _clienteRepository;
    private readonly IEmailService _emailService;

    public AgendamentoService(
        IGenericRepository<Agendamento> agendamentoRepository,
        IGenericRepository<Disponibilidade> disponibilidadeRepository,
        IGenericRepository<Servico> servicoRepository,
        IGenericRepository<Profissional> profissionalRepository,
        IGenericRepository<Cliente> clienteRepository,
        IMapper mapper,
        IEmailService emailService)
        : base(agendamentoRepository, mapper)
    {
        _agendamentoRepository = agendamentoRepository;
        _disponibilidadeRepository = disponibilidadeRepository;
        _servicoRepository = servicoRepository;
        _profissionalRepository = profissionalRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
    }

    public async Task<bool> ExisteConflitoHorario(int profissionalId, DateTime dataHoraInicio, DateTime dataHoraFim, int? agendamentoId = null)
    {
        var agendamentos = await _agendamentoRepository.FindAsync(a =>
            a.ProfissionalId == profissionalId &&
            a.Id != agendamentoId &&
            ((a.DataHoraInicio <= dataHoraInicio && a.DataHoraFim > dataHoraInicio) ||
             (a.DataHoraInicio < dataHoraFim && a.DataHoraFim >= dataHoraFim) ||
             (a.DataHoraInicio >= dataHoraInicio && a.DataHoraFim <= dataHoraFim)));

        return agendamentos.Any();
    }

    public async Task<bool> HorarioDentroDisponibilidade(int profissionalId, DateTime dataHoraInicio, DateTime dataHoraFim)
    {
        var disponibilidades = await _disponibilidadeRepository.FindAsync(d =>
            d.ProfissionalId == profissionalId &&
            d.Disponivel &&
            d.DataInicio <= dataHoraInicio &&
            d.DataFim >= dataHoraFim);

        return disponibilidades.Any();
    }

    public async Task<bool> ServicoAtivo(int servicoId)
    {
        var servico = await _servicoRepository.GetByIdAsync(servicoId);
        return servico != null && servico.Ativo;
    }

    public async Task<bool> ProfissionalAtivo(int profissionalId)
    {
        var profissional = await _profissionalRepository.GetByIdAsync(profissionalId);
        return profissional != null && profissional.Ativo;
    }

    public async Task<bool> ClienteAtivo(int clienteId)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        return cliente != null && cliente.Ativo;
    }

    public async Task<bool> ClienteAtingiuLimiteAgendamentosFuturos(int clienteId, int limite = 3)
    {
        var agora = DateTime.UtcNow;
        var agendamentosFuturos = await _agendamentoRepository.FindAsync(a => a.ClienteId == clienteId && a.DataHoraInicio > agora && !a.Cancelado);
        return agendamentosFuturos.Count() >= limite;
    }

    public async Task<bool> ServicoPertenceAoProfissional(int servicoId, int profissionalId)
    {
        var servico = await _servicoRepository.GetByIdAsync(servicoId);
        return servico != null && servico.ProfissionalId == profissionalId;
    }

    public async Task<bool> ClienteTemConflitoHorario(int clienteId, DateTime dataHoraInicio, DateTime dataHoraFim, int? agendamentoId = null)
    {
        var agendamentos = await _agendamentoRepository.FindAsync(a =>
            a.ClienteId == clienteId &&
            a.Id != agendamentoId &&
            ((a.DataHoraInicio <= dataHoraInicio && a.DataHoraFim > dataHoraInicio) ||
             (a.DataHoraInicio < dataHoraFim && a.DataHoraFim >= dataHoraFim) ||
             (a.DataHoraInicio >= dataHoraInicio && a.DataHoraFim <= dataHoraFim)));
        return agendamentos.Any();
    }

    public class ResultadoOperacaoAgendamento
    {
        public bool OperacaoValida { get; set; }
        public string Mensagem { get; set; }
        public AgendamentoDto AgendamentoDto { get; set; }
    }

    public override async Task<AgendamentoDto> CreateAsync(AgendamentoCreateUpdateDto dto)
    {
        var agendamento = _mapper.Map<Agendamento>(dto);
        agendamento.DataCriacao = DateTime.UtcNow;
        await _agendamentoRepository.AddAsync(agendamento);
        // Carregar dados completos para e-mail
        agendamento = await _agendamentoRepository.GetByIdAsync(agendamento.Id);
        var cliente = await _clienteRepository.GetByIdAsync(agendamento.ClienteId);
        var profissional = await _profissionalRepository.GetByIdAsync(agendamento.ProfissionalId);
        var servico = await _servicoRepository.GetByIdAsync(agendamento.ServicoId);
        // E-mail para cliente
        await _emailService.EnviarEmailAsync(
            cliente.Email,
            "Agendamento criado com sucesso",
            $"<p>Olá, {cliente.Nome}!</p><p>Seu agendamento para o serviço <b>{servico.Nome}</b> com o profissional <b>{profissional.Nome}</b> foi criado para o dia <b>{agendamento.DataHoraInicio:dd/MM/yyyy}</b> das <b>{agendamento.DataHoraInicio:HH:mm}</b> às <b>{agendamento.DataHoraFim:HH:mm}</b>.</p><p>Observações: {agendamento.Observacoes ?? "-"}</p>");
        // E-mail para profissional
        await _emailService.EnviarEmailAsync(
            profissional.Email,
            "Novo agendamento recebido",
            $"<p>Olá, {profissional.Nome}!</p><p>Você recebeu um novo agendamento para o serviço <b>{servico.Nome}</b> com o cliente <b>{cliente.Nome}</b> no dia <b>{agendamento.DataHoraInicio:dd/MM/yyyy}</b> das <b>{agendamento.DataHoraInicio:HH:mm}</b> às <b>{agendamento.DataHoraFim:HH:mm}</b>.</p><p>Observações: {agendamento.Observacoes ?? "-"}</p>");
        return _mapper.Map<AgendamentoDto>(agendamento);
    }

    public async Task<ResultadoOperacaoAgendamento> ConfirmarAgendamento(int id)
    {
        var agendamento = await _agendamentoRepository.GetByIdAsync(id);
        if (agendamento == null)
            return null;
        if (agendamento.Cancelado)
            return new ResultadoOperacaoAgendamento { OperacaoValida = false, Mensagem = "O agendamento já está cancelado." };
        if (agendamento.Confirmado)
            return new ResultadoOperacaoAgendamento { OperacaoValida = false, Mensagem = "O agendamento já está confirmado." };
        agendamento.Confirmado = true;
        agendamento.DataConfirmacao = DateTime.UtcNow;
        await _agendamentoRepository.UpdateAsync(agendamento);
        var cliente = await _clienteRepository.GetByIdAsync(agendamento.ClienteId);
        var profissional = await _profissionalRepository.GetByIdAsync(agendamento.ProfissionalId);
        var servico = await _servicoRepository.GetByIdAsync(agendamento.ServicoId);
        // E-mail para cliente
        await _emailService.EnviarEmailAsync(
            cliente.Email,
            "Agendamento confirmado",
            $"<p>Olá, {cliente.Nome}!</p><p>Seu agendamento para o serviço <b>{servico.Nome}</b> com o profissional <b>{profissional.Nome}</b> foi <b>confirmado</b> para o dia <b>{agendamento.DataHoraInicio:dd/MM/yyyy}</b> das <b>{agendamento.DataHoraInicio:HH:mm}</b> às <b>{agendamento.DataHoraFim:HH:mm}</b>.</p>");
        // E-mail para profissional
        await _emailService.EnviarEmailAsync(
            profissional.Email,
            "Agendamento confirmado",
            $"<p>Olá, {profissional.Nome}!</p><p>O agendamento para o serviço <b>{servico.Nome}</b> com o cliente <b>{cliente.Nome}</b> foi <b>confirmado</b> para o dia <b>{agendamento.DataHoraInicio:dd/MM/yyyy}</b> das <b>{agendamento.DataHoraInicio:HH:mm}</b> às <b>{agendamento.DataHoraFim:HH:mm}</b>.</p>");
        var dto = _mapper.Map<AgendamentoDto>(agendamento);
        return new ResultadoOperacaoAgendamento { OperacaoValida = true, AgendamentoDto = dto };
    }

    public async Task<ResultadoOperacaoAgendamento> CancelarAgendamento(int id)
    {
        var agendamento = await _agendamentoRepository.GetByIdAsync(id);
        if (agendamento == null)
            return null;
        if (agendamento.Cancelado)
            return new ResultadoOperacaoAgendamento { OperacaoValida = false, Mensagem = "O agendamento já está cancelado." };
        agendamento.Cancelado = true;
        agendamento.DataCancelamento = DateTime.UtcNow;
        await _agendamentoRepository.UpdateAsync(agendamento);
        var cliente = await _clienteRepository.GetByIdAsync(agendamento.ClienteId);
        var profissional = await _profissionalRepository.GetByIdAsync(agendamento.ProfissionalId);
        var servico = await _servicoRepository.GetByIdAsync(agendamento.ServicoId);
        // E-mail para cliente
        await _emailService.EnviarEmailAsync(
            cliente.Email,
            "Agendamento cancelado",
            $"<p>Olá, {cliente.Nome}!</p><p>Seu agendamento para o serviço <b>{servico.Nome}</b> com o profissional <b>{profissional.Nome}</b> foi <b>cancelado</b>. Se precisar, agende novamente pelo sistema.</p>");
        // E-mail para profissional
        await _emailService.EnviarEmailAsync(
            profissional.Email,
            "Agendamento cancelado",
            $"<p>Olá, {profissional.Nome}!</p><p>O agendamento para o serviço <b>{servico.Nome}</b> com o cliente <b>{cliente.Nome}</b> foi <b>cancelado</b>.</p>");
        var dto = _mapper.Map<AgendamentoDto>(agendamento);
        return new ResultadoOperacaoAgendamento { OperacaoValida = true, AgendamentoDto = dto };
    }
} 