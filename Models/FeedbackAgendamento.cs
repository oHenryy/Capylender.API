using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capylender.API.Models;

public class FeedbackAgendamento
{
    public int Id { get; set; }
    [Required]
    public int AgendamentoId { get; set; }
    [ForeignKey("AgendamentoId")]
    public Agendamento Agendamento { get; set; } = null!;
    [Required]
    [Range(1, 5)]
    public int Nota { get; set; }
    [StringLength(500)]
    public string? Comentario { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
} 