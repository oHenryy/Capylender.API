using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models.DTOs;

public class DisponibilidadeCreateUpdateDto
{
    [Required(ErrorMessage = "O profissional é obrigatório")]
    public int ProfissionalId { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória")]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória")]
    public DateTime DataFim { get; set; }

    public bool Disponivel { get; set; } = true;
} 