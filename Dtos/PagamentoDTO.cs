using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Dtos;

public class PagamentoDTO
{
    public int Id { get; set; }
    
    public DateTime Data { get; set; }

    public decimal Valor { get; set; }
    
    public string FormaPag { get; set; }
    
    public int CaixaId { get; set; }
    public CaixaDTO Caixa { get; set; }
    
    public int? CompraId { get; set; }
    public CompraDTO Compra { get; set; }
    
    public int? DespesaId { get; set; }
    public DespesaDTO Despesa { get; set; }
    
    public int FuncionarioId { get; set; }
    public FuncionarioDTO Funcionario { get; set; }
}

public class PagamentoCreateDTO : IValidatableObject
{
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
    [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
    public string FormaPag { get; set; }

    [Required(ErrorMessage = "Id do Caixa é obrigatório")]
    public int CaixaId { get; set; }

    public int? CompraId { get; set; }

    public int? DespesaId { get; set; }

    [Required(ErrorMessage = "Id do Funcionário é obrigatório")]
    public int FuncionarioId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

        var caixa = context.Caixas.Find(CaixaId);
        if (caixa == null)
            yield return new ValidationResult("Caixa não encontrado.", new[] { nameof(CaixaId) });
        else if (!caixa.Status)
            yield return new ValidationResult("Caixa fechado. Não é possível registrar pagamentos.", new[] { nameof(CaixaId) });

        if (!context.Funcionarios.Any(f => f.Id == FuncionarioId))
            yield return new ValidationResult("Funcionário não encontrado.", new[] { nameof(FuncionarioId) });

        if (!CompraId.HasValue && !DespesaId.HasValue)
            yield return new ValidationResult("É necessário informar uma Compra ou Despesa para o pagamento.", new[] { nameof(CompraId), nameof(DespesaId) });

        if (CompraId.HasValue && !context.Compras.Any(c => c.Id == CompraId.Value))
            yield return new ValidationResult("Compra não encontrada.", new[] { nameof(CompraId) });

        if (DespesaId.HasValue && !context.Despesas.Any(d => d.Id == DespesaId.Value))
            yield return new ValidationResult("Despesa não encontrada.", new[] { nameof(DespesaId) });
    }
}

public class PagamentoUpdateDTO
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal? Valor { get; set; }

    [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
    public string? FormaPag { get; set; }

    public int? CaixaId { get; set; }

    public int? CompraId { get; set; }

    public int? DespesaId { get; set; }

    public int? FuncionarioId { get; set; }
}