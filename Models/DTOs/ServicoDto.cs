namespace Capylender.API.Models.DTOs;

public class ServicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal Preco { get; set; }
    public int DuracaoMinutos { get; set; }
    public bool Ativo { get; set; }
    public int ProfissionalId { get; set; }
    public string? ProfissionalNome { get; set; }
} 