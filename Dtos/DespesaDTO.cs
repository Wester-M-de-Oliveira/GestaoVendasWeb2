using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos;
public class DespesaDTO
{
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A data de vencimento é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataVencimento { get; set; }

    [Required(ErrorMessage = "O número do documento é obrigatório")]
    public int NumeroDoc { get; set; }

    [Required(ErrorMessage = "O fornecedor é obrigatório")]
    public int FornecedorId { get; set; }

    public FornecedorDTO Fornecedor { get; set; }
    public List<PagamentoDTO> Pagamentos { get; set; } = new();
}
public class DespesaCreateDTO
{
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A data de vencimento é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataVencimento { get; set; }

    [Required(ErrorMessage = "O número do documento é obrigatório")]
    public int NumeroDoc { get; set; }

    [Required(ErrorMessage = "O fornecedor é obrigatório")]
    public int FornecedorId { get; set; }
}

public class DespesaUpdateDTO
{
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal? Valor { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataVencimento { get; set; }

    public int? NumeroDoc { get; set; }

    public int? FornecedorId { get; set; }
}