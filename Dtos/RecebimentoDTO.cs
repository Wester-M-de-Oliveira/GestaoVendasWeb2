using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GestaoVendasWeb2.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Dtos
{
    public class RecebimentoDTO
    {
        public int Id { get; set; }

        public DateTime Data { get; set; }

        public decimal Valor { get; set; }


        public int CaixaId { get; set; }
        public CaixaDTO Caixa { get; set; }

        public int VendaId { get; set; }
        public VendaDTO Venda { get; set; }

        public int FuncionarioId { get; set; }
        public FuncionarioDTO Funcionario { get; set; }
        
        // Propriedade calculada para mostrar o valor pendente da venda após este recebimento
        public decimal ValorPendente => Venda?.Valor - Venda?.Recebimentos?.Sum(r => r.Valor) ?? 0;
    }
    
    public class RecebimentoCreateDTO : IValidatableObject
    {
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O caixa é obrigatório")]
        public int CaixaId { get; set; }

        [Required(ErrorMessage = "A venda é obrigatória")]
        public int VendaId { get; set; }

        [Required(ErrorMessage = "O funcionário é obrigatório")]
        public int FuncionarioId { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            
            // Verificar se o caixa existe e está aberto
            var caixa = context.Caixas.Find(CaixaId);
            if (caixa == null)
                yield return new ValidationResult("Caixa não encontrado.", [nameof(CaixaId)]);
            else if (!caixa.Status)
                yield return new ValidationResult("O caixa está fechado. Não é possível registrar recebimentos.", [nameof(CaixaId)]);
            
            // Verificar se a venda existe
            var venda = context.Vendas
                .FirstOrDefault(v => v.Id == VendaId);
            
            if (venda == null)
                yield return new ValidationResult("Venda não encontrada.", [nameof(VendaId)]);
            else
            {
                // Carregar recebimentos da venda para calcular valor pendente
                context.Entry(venda).Collection(v => v.Recebimentos).Load();
                
                // Calcular valor pendente
                var valorTotalPago = venda.Recebimentos.Sum(r => r.Valor);
                var valorPendente = venda.Valor - valorTotalPago;
                
                // Verificar se o valor do recebimento não é maior que o valor pendente
                if (Valor > valorPendente)
                    yield return new ValidationResult($"Valor do recebimento (R$ {Valor}) maior que o valor pendente (R$ {valorPendente}).", [nameof(Valor)]);
            }
            
            // Verificar se o funcionário existe
            if (!context.Funcionarios.Any(f => f.Id == FuncionarioId))
                yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);
        }
    }
    
    public class RecebimentoUpdateDTO : IValidatableObject
    {
        public int Id { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal? Valor { get; set; }
        
        public int? CaixaId { get; set; }
        
        public int? FuncionarioId { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            
            var recebimento = context.Recebimentos
                .Include(r => r.Venda)
                .Include(r => r.Venda.Recebimentos)
                .Include(r => r.Caixa)
                .FirstOrDefault(r => r.Id == Id);
                
            if (recebimento == null)
                yield break; // A validação de existência será feita no controller
            
            // Verificar se o caixa do recebimento está aberto
            if (!recebimento.Caixa.Status)
                yield return new ValidationResult("Não é possível modificar um recebimento associado a um caixa fechado.", [nameof(Id)]);
            
            // Se estiver alterando para outro caixa, verificar se o novo caixa está aberto
            if (CaixaId.HasValue && CaixaId.Value != recebimento.CaixaId)
            {
                var novoCaixa = context.Caixas.Find(CaixaId.Value);
                if (novoCaixa == null)
                    yield return new ValidationResult("Caixa não encontrado.", [nameof(CaixaId)]);
                else if (!novoCaixa.Status)
                    yield return new ValidationResult("O caixa está fechado. Não é possível transferir o recebimento.", [nameof(CaixaId)]);
            }
            
            // Verificar se o novo valor não excede o valor pendente da venda
            if (Valor.HasValue)
            {
                var valorTotalAtual = recebimento.Venda.Recebimentos
                    .Where(r => r.Id != recebimento.Id)
                    .Sum(r => r.Valor);
                    
                var valorDisponivel = recebimento.Venda.Valor - valorTotalAtual;
                
                if (Valor.Value > valorDisponivel)
                    yield return new ValidationResult($"Valor do recebimento (R$ {Valor}) maior que o valor disponível (R$ {valorDisponivel}).", [nameof(Valor)]);
            }
            
            // Verificar se o funcionário existe
            if (FuncionarioId.HasValue && !context.Funcionarios.Any(f => f.Id == FuncionarioId.Value))
                yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);
        }
    }
}