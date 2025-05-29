using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Services;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfissionaisController : ControllerBase
{
    private readonly IGenericService<Profissional, ProfissionalDto, ProfissionalCreateUpdateDto> _service;

    public ProfissionaisController(IGenericService<Profissional, ProfissionalDto, ProfissionalCreateUpdateDto> service)
    {
        _service = service;
    }

    // GET: api/profissionais
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfissionalDto>>> GetProfissionais([FromQuery] bool? ativos = null)
    {
        var profissionais = await _service.GetAllAsync();
        if (ativos.HasValue)
        {
            profissionais = profissionais.Where(p => p.Ativo == ativos.Value);
        }
        return Ok(profissionais);
    }

    // GET: api/profissionais/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfissionalDto>> GetProfissional(int id)
    {
        var profissional = await _service.GetByIdAsync(id);
        if (profissional == null)
            return NotFound();
        return Ok(profissional);
    }

    // POST: api/profissionais
    [HttpPost]
    public async Task<ActionResult<ProfissionalDto>> PostProfissional(ProfissionalCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var profissional = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProfissional), new { id = profissional.Id }, profissional);
    }

    // PUT: api/profissionais/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProfissional(int id, ProfissionalCreateUpdateDto dto)
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

    // DELETE: api/profissionais/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfissional(int id)
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

    // PATCH: api/profissionais/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool ativo)
    {
        try
        {
            var profissional = await _service.GetByIdAsync(id);
            if (profissional == null)
                return NotFound();

            var dto = new ProfissionalCreateUpdateDto
            {
                Nome = profissional.Nome,
                Email = profissional.Email,
                Telefone = profissional.Telefone,
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