namespace Capylender.API.Models.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string CPF { get; set; } = null!;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
} 