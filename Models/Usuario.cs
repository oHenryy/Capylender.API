using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capylender.API.Models;

public class Usuario
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
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Cliente"; // Cliente, Profissional, Admin

    // Relacionamentos opcionais
    public int? ClienteId { get; set; }
    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }

    public int? ProfissionalId { get; set; }
    [ForeignKey("ProfissionalId")]
    public Profissional? Profissional { get; set; }

    public bool Bloqueado { get; set; } = false;
} 