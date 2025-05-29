using System;

namespace Capylender.API.Models.DTOs;

public class AgendamentoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ProfissionalId { get; set; }
    public int ServicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public string? Observacoes { get; set; }
    public bool Confirmado { get; set; }
    public bool Cancelado { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataConfirmacao { get; set; }
    public DateTime? DataCancelamento { get; set; }
} 