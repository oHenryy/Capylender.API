using System;
using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models.DTOs;

public class AgendamentoCreateUpdateDto
{
    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int ProfissionalId { get; set; }

    [Required]
    public int ServicoId { get; set; }

    [Required]
    public DateTime DataHoraInicio { get; set; }

    [Required]
    public DateTime DataHoraFim { get; set; }

    public string? Observacoes { get; set; }
} 