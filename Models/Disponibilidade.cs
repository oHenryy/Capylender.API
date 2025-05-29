using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capylender.API.Models;

public class Disponibilidade
{
    public int Id { get; set; }

    [Required]
    public int ProfissionalId { get; set; }

    [ForeignKey("ProfissionalId")]
    public Profissional Profissional { get; set; } = null!;

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    public bool Disponivel { get; set; } = true;
} 