using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capylender.API.Models;

public class Servico
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Descricao { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Required]
    public int DuracaoMinutos { get; set; }

    public bool Ativo { get; set; } = true;

    [Required]
    public int ProfissionalId { get; set; }

    [ForeignKey("ProfissionalId")]
    public Profissional Profissional { get; set; } = null!;

    // Relacionamento com agendamentos
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
} 