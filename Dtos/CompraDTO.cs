using System.ComponentModel.DataAnnotations;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.DataContexts;
using System.Text.Json.Serialization;
public class CompraDTO
{
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

    public List<ItensCompraDTO> ItensCompra { get; set; } = [];

    public List<PagamentoDTO> Pagamentos { get; set; } = [];
}

public class CompraCreateDto : IValidatableObject
{
    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Data { get; set; }

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

    [Required(ErrorMessage = "Itens da compra são obrigatórios")]
    public List<ItensCompraDTO> ItensCompra { get; set; } = [];

    [JsonIgnore]
    public decimal Valor { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

        if (!_context.Funcionarios.Any(f => f.Id == FuncionarioId))
            yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);

        if (!_context.Fornecedores.Any(f => f.Id == FornecedorId))
            yield return new ValidationResult("Fornecedor não encontrado.", [nameof(FornecedorId)]);

        if (ItensCompra == null || !ItensCompra.Any())
            yield return new ValidationResult("A compra deve ter pelo menos um item.", [nameof(ItensCompra)]);

        var produtoIds = ItensCompra.Select(i => i.ProdutoId).ToList();
        var produtosExistentes = _context.Produtos.Where(p => produtoIds.Contains(p.Id)).Select(p => p.Id).ToList();
        var produtosNaoEncontrados = produtoIds.Except(produtosExistentes).ToList();

        if (produtosNaoEncontrados.Any())
            yield return new ValidationResult($"Os seguintes produtos não foram encontrados: {string.Join(", ", produtosNaoEncontrados)}", [nameof(ItensCompra)]);

        Valor = 0;

        foreach (var item in ItensCompra)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.Id == item.ProdutoId);

            if (produto == null)
            {
                yield return new ValidationResult($"Produto com ID {item.ProdutoId} não encontrado.", [nameof(ItensCompra)]);
                continue;
            }

            if (produto.PrecoCompra == 0)
            {
                yield return new ValidationResult($"Produto com ID {item.ProdutoId} não possui preço de compra definido.", [nameof(ItensCompra)]);
                continue;
            }

            Valor += item.Quantidade * produto.PrecoCompra;
        }
    }
}

public class CompraUpdateDto : IValidatableObject
{
    public DateTime? Data { get; set; }

    [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
    public string? FormaPag { get; set; }

    public StatusCompra? Status { get; set; }

    [StringLength(200, ErrorMessage = "Observação deve ter no máximo 200 caracteres")]
    public string? Observacao { get; set; }

    public int? FuncionarioId { get; set; }

    public int? FornecedorId { get; set; }

    public List<ItensCompraDTO>? ItensCompra { get; set; }

    [JsonIgnore]
    public decimal Valor { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

        if (FuncionarioId.HasValue && !_context.Funcionarios.Any(f => f.Id == FuncionarioId))
            yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);

        if (FornecedorId.HasValue && !_context.Fornecedores.Any(f => f.Id == FornecedorId))
            yield return new ValidationResult("Fornecedor não encontrado.", [nameof(FornecedorId)]);

        if (ItensCompra != null && ItensCompra.Any())
        {
            var produtoIds = ItensCompra.Select(i => i.ProdutoId).ToList();
            var produtosExistentes = _context.Produtos.Where(p => produtoIds.Contains(p.Id)).Select(p => p.Id).ToList();
            var produtosNaoEncontrados = produtoIds.Except(produtosExistentes).ToList();

            if (produtosNaoEncontrados.Any())
                yield return new ValidationResult($"Os seguintes produtos não foram encontrados: {string.Join(", ", produtosNaoEncontrados)}", [nameof(ItensCompra)]);
        }
    }
}

public class ItensCompraDTO
{
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "Produto é obrigatório")]
    public int ProdutoId { get; set; }
}