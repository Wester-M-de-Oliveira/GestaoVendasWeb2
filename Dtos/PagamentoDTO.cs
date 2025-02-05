using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos;

public class PagamentoDTO
{
    [Required(ErrorMessage = "Id é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public double Valor { get; set; }

    [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
    [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
    public string FormaPag { get; set; }

    [Required(ErrorMessage = "Id do Caixa é obrigatório")]
    public int CaixaId { get; set; }

    [Required(ErrorMessage = "Id da Compra é obrigatório")]
    public int CompraId { get; set; }

    [Required(ErrorMessage = "Id da Despesa é obrigatório")]
    public int DespesaId { get; set; }

    [Required(ErrorMessage = "Id do Funcionário é obrigatório")]
    public int FuncionarioId { get; set; }
}