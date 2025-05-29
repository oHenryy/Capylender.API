using Microsoft.AspNetCore.Mvc;
using Capylender.API.Data;
using Capylender.API.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Capylender.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackAgendamentoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FeedbackAgendamentoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarFeedback([FromBody] FeedbackAgendamentoDto dto)
    {
        var agendamento = await _context.Agendamentos.FindAsync(dto.AgendamentoId);
        if (agendamento == null)
            return NotFound("Agendamento não encontrado.");
        if (await _context.FeedbacksAgendamento.AnyAsync(f => f.AgendamentoId == dto.AgendamentoId))
            return BadRequest("Feedback já registrado para este agendamento.");
        var feedback = new FeedbackAgendamento
        {
            AgendamentoId = dto.AgendamentoId,
            Nota = dto.Nota,
            Comentario = dto.Comentario
        };
        _context.FeedbacksAgendamento.Add(feedback);
        await _context.SaveChangesAsync();
        return Ok(new { mensagem = "Feedback registrado com sucesso." });
    }
}

public class FeedbackAgendamentoDto
{
    public int AgendamentoId { get; set; }
    public int Nota { get; set; }
    public string? Comentario { get; set; }
} 