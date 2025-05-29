using System;
using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models;

public class LogAuditoria
{
    public int Id { get; set; }
    [Required]
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    [Required]
    public string Usuario { get; set; } = string.Empty;
    [Required]
    public string Acao { get; set; } = string.Empty;
    public string? Detalhes { get; set; }
} 