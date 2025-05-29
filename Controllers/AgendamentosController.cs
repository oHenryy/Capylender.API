using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Capylender.API.Models.DTOs;
using Capylender.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Capylender.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os agendamentos do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AgendamentosController : ControllerBase
{
    private readonly AgendamentoService _agendamentoService;
    private readonly IMapper _mapper;

    public AgendamentosController(AgendamentoService agendamentoService, IMapper mapper)
    {
        _agendamentoService = agendamentoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna todos os agendamentos cadastrados.
    /// </summary>
    /// <returns>Lista de agendamentos</returns>
    /// <response code="200">Retorna a lista de agendamentos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetAgendamentos()
    {
        var agendamentos = await _agendamentoService.GetAllAsync();
        return Ok(agendamentos);
    }

    /// <summary>
    /// Retorna um agendamento específico pelo ID.
    /// </summary>
    /// <param name="id">ID do agendamento</param>
    /// <returns>Dados do agendamento</returns>
    /// <response code="200">Retorna o agendamento solicitado</response>
    /// <response code="404">Se o agendamento não for encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgendamentoDto>> GetAgendamento(int id)
    {
        var agendamento = await _agendamentoService.GetByIdAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }
        return Ok(agendamento);
    }

    /// <summary>
    /// Cria um novo agendamento.
    /// </summary>
    /// <param name="dto">Dados do agendamento</param>
    /// <returns>Agendamento criado</returns>
    /// <response code="201">Retorna o agendamento criado</response>
    /// <response code="400">Se os dados do agendamento forem inválidos</response>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/Agendamentos
    ///     {
    ///        "clienteId": 1,
    ///        "profissionalId": 1,
    ///        "servicoId": 1,
    ///        "dataHoraInicio": "2024-03-20T10:00:00",
    ///        "dataHoraFim": "2024-03-20T11:00:00",
    ///        "observacoes": "Cliente preferencial"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AgendamentoDto>> PostAgendamento(AgendamentoCreateUpdateDto dto)
    {
        if (!await _agendamentoService.ClienteAtivo(dto.ClienteId))
        {
            return BadRequest("O cliente selecionado está inativo.");
        }
        if (dto.DataHoraInicio < DateTime.UtcNow)
        {
            return BadRequest("A data/hora de início não pode estar no passado.");
        }
        if (dto.DataHoraFim <= dto.DataHoraInicio)
        {
            return BadRequest("A data/hora de fim deve ser maior que a de início.");
        }

        if (await _agendamentoService.ExisteConflitoHorario(dto.ProfissionalId, dto.DataHoraInicio, dto.DataHoraFim))
        {
            return BadRequest("Já existe um agendamento para este horário.");
        }

        if (!await _agendamentoService.HorarioDentroDisponibilidade(dto.ProfissionalId, dto.DataHoraInicio, dto.DataHoraFim))
        {
            return BadRequest("O horário do agendamento está fora das disponibilidades do profissional.");
        }

        if (!await _agendamentoService.ServicoAtivo(dto.ServicoId))
        {
            return BadRequest("O serviço selecionado está inativo.");
        }

        if (!await _agendamentoService.ProfissionalAtivo(dto.ProfissionalId))
        {
            return BadRequest("O profissional selecionado está inativo.");
        }

        if (!await _agendamentoService.ServicoPertenceAoProfissional(dto.ServicoId, dto.ProfissionalId))
        {
            return BadRequest("O serviço não pertence ao profissional selecionado.");
        }

        if (await _agendamentoService.ClienteAtingiuLimiteAgendamentosFuturos(dto.ClienteId))
        {
            return BadRequest("O cliente já atingiu o limite de agendamentos futuros permitidos.");
        }

        if (await _agendamentoService.ClienteTemConflitoHorario(dto.ClienteId, dto.DataHoraInicio, dto.DataHoraFim))
        {
            return BadRequest("O cliente já possui um agendamento para este horário.");
        }

        var agendamento = await _agendamentoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAgendamento), new { id = agendamento.Id }, agendamento);
    }

    /// <summary>
    /// Atualiza um agendamento existente.
    /// </summary>
    /// <param name="id">ID do agendamento</param>
    /// <param name="dto">Novos dados do agendamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Se o agendamento for atualizado com sucesso</response>
    /// <response code="400">Se os dados do agendamento forem inválidos</response>
    /// <response code="404">Se o agendamento não for encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAgendamento(int id, AgendamentoCreateUpdateDto dto)
    {
        if (!await _agendamentoService.ClienteAtivo(dto.ClienteId))
        {
            return BadRequest("O cliente selecionado está inativo.");
        }
        if (dto.DataHoraInicio < DateTime.UtcNow)
        {
            return BadRequest("A data/hora de início não pode estar no passado.");
        }
        if (dto.DataHoraFim <= dto.DataHoraInicio)
        {
            return BadRequest("A data/hora de fim deve ser maior que a de início.");
        }

        if (await _agendamentoService.ExisteConflitoHorario(dto.ProfissionalId, dto.DataHoraInicio, dto.DataHoraFim, id))
        {
            return BadRequest("Já existe um agendamento para este horário.");
        }

        if (!await _agendamentoService.HorarioDentroDisponibilidade(dto.ProfissionalId, dto.DataHoraInicio, dto.DataHoraFim))
        {
            return BadRequest("O horário do agendamento está fora das disponibilidades do profissional.");
        }

        if (!await _agendamentoService.ServicoAtivo(dto.ServicoId))
        {
            return BadRequest("O serviço selecionado está inativo.");
        }

        if (!await _agendamentoService.ProfissionalAtivo(dto.ProfissionalId))
        {
            return BadRequest("O profissional selecionado está inativo.");
        }

        if (!await _agendamentoService.ServicoPertenceAoProfissional(dto.ServicoId, dto.ProfissionalId))
        {
            return BadRequest("O serviço não pertence ao profissional selecionado.");
        }

        await _agendamentoService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Remove um agendamento.
    /// </summary>
    /// <param name="id">ID do agendamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Se o agendamento for removido com sucesso</response>
    /// <response code="404">Se o agendamento não for encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAgendamento(int id)
    {
        await _agendamentoService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Confirma um agendamento.
    /// </summary>
    /// <param name="id">ID do agendamento</param>
    /// <returns>Agendamento confirmado</returns>
    /// <response code="200">Agendamento confirmado com sucesso</response>
    /// <response code="400">Se o agendamento já estiver confirmado ou cancelado</response>
    /// <response code="404">Se o agendamento não for encontrado</response>
    [HttpPost("{id}/confirmar")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmarAgendamento(int id)
    {
        var resultado = await _agendamentoService.ConfirmarAgendamento(id);
        if (resultado == null)
            return NotFound();
        if (!resultado.OperacaoValida)
            return BadRequest(resultado.Mensagem);
        return Ok(resultado.AgendamentoDto);
    }

    /// <summary>
    /// Cancela um agendamento.
    /// </summary>
    /// <param name="id">ID do agendamento</param>
    /// <returns>Agendamento cancelado</returns>
    /// <response code="200">Agendamento cancelado com sucesso</response>
    /// <response code="400">Se o agendamento já estiver cancelado</response>
    /// <response code="404">Se o agendamento não for encontrado</response>
    [HttpPost("{id}/cancelar")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelarAgendamento(int id)
    {
        var resultado = await _agendamentoService.CancelarAgendamento(id);
        if (resultado == null)
            return NotFound();
        if (!resultado.OperacaoValida)
            return BadRequest(resultado.Mensagem);
        return Ok(resultado.AgendamentoDto);
    }
} 