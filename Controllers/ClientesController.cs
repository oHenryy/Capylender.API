using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capylender.API.Models;
using Capylender.API.Models.DTOs;
using Capylender.API.Services;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IGenericService<Cliente, ClienteDto, ClienteCreateUpdateDto> _service;

    public ClientesController(IGenericService<Cliente, ClienteDto, ClienteCreateUpdateDto> service)
    {
        _service = service;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
    {
        var clientes = await _service.GetAllAsync();
        return Ok(clientes);
    }

    // GET: api/clientes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetCliente(int id)
    {
        var cliente = await _service.GetByIdAsync(id);
        if (cliente == null)
            return NotFound();
        return Ok(cliente);
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> PostCliente(ClienteCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cliente = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
    }

    // PUT: api/clientes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, ClienteCreateUpdateDto dto)
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

    // DELETE: api/clientes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
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
} 