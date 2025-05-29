namespace Capylender.API.Models.DTOs;

public class DisponibilidadeDto
{
    public int Id { get; set; }
    public int ProfissionalId { get; set; }
    public string? ProfissionalNome { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Disponivel { get; set; }
} 