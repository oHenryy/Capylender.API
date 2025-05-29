using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models.DTOs;

public class ProfissionalCreateUpdateDto
{
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
} 