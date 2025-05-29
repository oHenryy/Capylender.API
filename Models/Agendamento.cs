using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capylender.API.Models;

public class Agendamento
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [ForeignKey("ClienteId")]
    public Cliente Cliente { get; set; } = null!;

    [Required]
    public int ProfissionalId { get; set; }

    [ForeignKey("ProfissionalId")]
    public Profissional Profissional { get; set; } = null!;

    [Required]
    public int ServicoId { get; set; }

    [ForeignKey("ServicoId")]
    public Servico Servico { get; set; } = null!;

    [Required]
    public DateTime DataHoraInicio { get; set; }

    [Required]
    public DateTime DataHoraFim { get; set; }

    public string? Observacoes { get; set; }

    public bool Confirmado { get; set; }

    public bool Cancelado { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataConfirmacao { get; set; }

    public DateTime? DataCancelamento { get; set; }
}

public enum StatusAgendamento
{
    Agendado,
    Confirmado,
    Cancelado,
    Concluido,
    NoShow
} 