using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Services;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicosController : ControllerBase
{
    private readonly IServicoService _service;

    public ServicosController(IServicoService service)
    {
        _service = service;
    }

    // GET: api/servicos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServicoDto>>> GetServicos([FromQuery] bool? ativos = null)
    {
        var servicos = ativos.HasValue && ativos.Value
            ? await _service.GetServicosAtivosAsync()
            : await _service.GetAllAsync();
        return Ok(servicos);
    }

    // GET: api/servicos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServicoDto>> GetServico(int id)
    {
        var servico = await _service.GetByIdAsync(id);
        if (servico == null)
            return NotFound();
        return Ok(servico);
    }

    // GET: api/servicos/profissional/5
    [HttpGet("profissional/{profissionalId}")]
    public async Task<ActionResult<IEnumerable<ServicoDto>>> GetServicosByProfissional(int profissionalId)
    {
        var servicos = await _service.GetServicosByProfissionalAsync(profissionalId);
        return Ok(servicos);
    }

    // POST: api/servicos
    [HttpPost]
    public async Task<ActionResult<ServicoDto>> PostServico(ServicoCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var servico = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetServico), new { id = servico.Id }, servico);
    }

    // PUT: api/servicos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutServico(int id, ServicoCreateUpdateDto dto)
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

    // DELETE: api/servicos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServico(int id)
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

    // PATCH: api/servicos/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool ativo)
    {
        try
        {
            var servico = await _service.GetByIdAsync(id);
            if (servico == null)
                return NotFound();

            var dto = new ServicoCreateUpdateDto
            {
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Preco = servico.Preco,
                DuracaoMinutos = servico.DuracaoMinutos,
                ProfissionalId = servico.ProfissionalId,
                Ativo = ativo
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