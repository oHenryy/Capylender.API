using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Services;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Profissional")]
public class DisponibilidadesController : ControllerBase
{
    private readonly IDisponibilidadeService _service;

    public DisponibilidadesController(IDisponibilidadeService service)
    {
        _service = service;
    }

    // GET: api/disponibilidades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DisponibilidadeDto>>> GetDisponibilidades(
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null)
    {
        if (inicio.HasValue && fim.HasValue)
        {
            var disponibilidades = await _service.GetDisponibilidadesByPeriodoAsync(inicio.Value, fim.Value);
            return Ok(disponibilidades);
        }

        var todasDisponibilidades = await _service.GetAllAsync();
        return Ok(todasDisponibilidades);
    }

    // GET: api/disponibilidades/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DisponibilidadeDto>> GetDisponibilidade(int id)
    {
        var disponibilidade = await _service.GetByIdAsync(id);
        if (disponibilidade == null)
            return NotFound();
        return Ok(disponibilidade);
    }

    // GET: api/disponibilidades/profissional/5
    [HttpGet("profissional/{profissionalId}")]
    public async Task<ActionResult<IEnumerable<DisponibilidadeDto>>> GetDisponibilidadesByProfissional(
        int profissionalId,
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null)
    {
        if (inicio.HasValue && fim.HasValue)
        {
            var disponibilidades = await _service.GetDisponibilidadesByProfissionalAndPeriodoAsync(profissionalId, inicio.Value, fim.Value);
            return Ok(disponibilidades);
        }

        var todasDisponibilidades = await _service.GetDisponibilidadesByProfissionalAsync(profissionalId);
        return Ok(todasDisponibilidades);
    }

    // POST: api/disponibilidades
    [HttpPost]
    public async Task<ActionResult<DisponibilidadeDto>> PostDisponibilidade(DisponibilidadeCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var disponibilidade = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetDisponibilidade), new { id = disponibilidade.Id }, disponibilidade);
    }

    // PUT: api/disponibilidades/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDisponibilidade(int id, DisponibilidadeCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE: api/disponibilidades/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDisponibilidade(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PATCH: api/disponibilidades/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool disponivel)
    {
        try
        {
            var disponibilidade = await _service.GetByIdAsync(id);
            if (disponibilidade == null)
                return NotFound();

            var dto = new DisponibilidadeCreateUpdateDto
            {
                ProfissionalId = disponibilidade.ProfissionalId,
                DataInicio = disponibilidade.DataInicio,
                DataFim = disponibilidade.DataFim,
                Disponivel = disponivel
            };

            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 