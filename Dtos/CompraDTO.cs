using System.ComponentModel.DataAnnotations;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

public class CompraDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
    [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
    public string FormaPag { get; set; }

    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusCompra Status { get; set; }

    [StringLength(200, ErrorMessage = "Observação deve ter no máximo 200 caracteres")]
    public string Observacao { get; set; }

    [Required(ErrorMessage = "Funcionário é obrigatório")]
    public int FuncionarioId { get; set; }

    [Required(ErrorMessage = "Fornecedor é obrigatório")]
    public int FornecedorId { get; set; }

    public FuncionarioDTO Funcionario { get; set; }
    public FornecedorDTO Fornecedor { get; set; }
    public List<ItensCompraDTO> ItensCompra { get; set; }
    public List<PagamentoDTO> Pagamentos { get; set; }
}

public class ItensCompraDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public double Valor { get; set; }

    [Required(ErrorMessage = "Produto é obrigatório")]
    public Produto ProdutoId { get; set; }

    [Required(ErrorMessage = "Compra é obrigatória")]
    public Compra CompraId { get; set; }
}