using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models;

public class Profissional
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Telefone { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public ICollection<Disponibilidade> Disponibilidades { get; set; } = new List<Disponibilidade>();
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
} 