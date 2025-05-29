using System.ComponentModel.DataAnnotations;

namespace Capylender.API.Models.DTOs;

public class ServicoCreateUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, 999999.99, ErrorMessage = "O preço deve estar entre 0,01 e 999.999,99")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "A duração é obrigatória")]
    [Range(1, 480, ErrorMessage = "A duração deve estar entre 1 e 480 minutos")]
    public int DuracaoMinutos { get; set; }

    public bool Ativo { get; set; } = true;

    [Required(ErrorMessage = "O profissional é obrigatório")]
    public int ProfissionalId { get; set; }
} 