using System.ComponentModel.DataAnnotations;
using GestaoVendasWeb2.Models;
using System.Text.Json.Serialization;
using GestaoVendasWeb2.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Dtos
{
    public class VendaDTO
    {
        public int Id { get; set; }

        public DateTime DataVenda { get; set; }

        public decimal Valor { get; set; }

        public decimal? Desconto { get; set; }

        public string FormaPag { get; set; }

        public int? QuantParcelas { get; set; }

        public int FuncionarioId { get; set; }
        public FuncionarioDTO Funcionario { get; set; }

        public int ClienteId { get; set; }
        public ClienteDTO Cliente { get; set; }

        public List<ItensVendaDTO> ItensVendas { get; set; } = new List<ItensVendaDTO>();
        
        public List<RecebimentoDTO> Recebimentos { get; set; } = new List<RecebimentoDTO>();
        
        // Propriedade calculada para mostrar o valor restante a ser pago
        public decimal ValorRestante => Valor - Recebimentos.Sum(r => r.Valor);
    }

    public class VendaCreateDTO : IValidatableObject
    {
        [Display(Name = "Desconto")]
        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal? Desconto { get; set; }

        [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
        [Display(Name = "Forma de Pagamento")]
        [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
        public string FormaPag { get; set; }

        [Display(Name = "Quantidade de Parcelas")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de parcelas não pode ser negativa")]
        public int? QuantParcelas { get; set; }

        [Required(ErrorMessage = "O funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "A venda deve conter pelo menos um item")]
        [MinLength(1, ErrorMessage = "A venda deve conter pelo menos um item")]
        public List<ItensVendaCreateDTO> ItensVendas { get; set; } = new List<ItensVendaCreateDTO>();
        
        [JsonIgnore]
        public decimal ValorTotal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            // Validar se o funcionário existe
            if (!context.Funcionarios.Any(f => f.Id == FuncionarioId))
                yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);

            // Validar se o cliente existe
            if (!context.Clientes.Any(c => c.Id == ClienteId))
                yield return new ValidationResult("Cliente não encontrado.", [nameof(ClienteId)]);

            // Validar se todos os itens da venda possuem produtos válidos
            if (ItensVendas == null || !ItensVendas.Any())
                yield return new ValidationResult("A venda deve ter pelo menos um item.", [nameof(ItensVendas)]);

            // Validar se as quantidades são positivas
            foreach (var item in ItensVendas)
            {
                if (item.Quantidade <= 0)
                    yield return new ValidationResult($"A quantidade do item deve ser maior que zero.", [nameof(ItensVendas)]);
            }

            // Pegar todos os IDs de produtos para validação em lote
            var produtoIds = ItensVendas.Select(i => i.ProdutoId).ToList();
            
            // Buscar produtos existentes no banco
            var produtos = context.Produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToDictionary(p => p.Id, p => p);
            
            // Verificar quais produtos não foram encontrados
            var produtosNaoEncontrados = produtoIds
                .Where(id => !produtos.ContainsKey(id))
                .ToList();

            // Reportar produtos não encontrados
            if (produtosNaoEncontrados.Any())
                yield return new ValidationResult(
                    $"Os seguintes produtos não foram encontrados: {string.Join(", ", produtosNaoEncontrados)}", 
                    [nameof(ItensVendas)]);

            // Verificar estoque e calcular o valor total
            ValorTotal = 0;
            foreach (var item in ItensVendas)
            {
                if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                    continue; // Produto não encontrado, já reportado acima
                    
                // Verificar estoque
                if (produto.QuantidadeEstoque < item.Quantidade)
                    yield return new ValidationResult(
                        $"Produto {produto.Descricao} (ID: {produto.Id}) possui apenas {produto.QuantidadeEstoque} unidades em estoque, mas foram solicitadas {item.Quantidade}.", 
                        [nameof(ItensVendas)]);
                
                // Atribuir o valor correto do produto e calcular o subtotal
                item.Valor = produto.Valor;
                ValorTotal += item.Quantidade * produto.Valor;
            }

            // Aplicar desconto se houver
            if (Desconto.HasValue)
            {
                if (Desconto > ValorTotal)
                    yield return new ValidationResult("O desconto não pode ser maior que o valor total da venda.", [nameof(Desconto)]);
                else
                    ValorTotal -= Desconto.Value;
            }

            // Validar forma de pagamento e parcelas
            if (FormaPag.ToLower() == "parcelado" || FormaPag.ToLower() == "crediário")
            {
                if (!QuantParcelas.HasValue || QuantParcelas <= 0)
                    yield return new ValidationResult("Para pagamento parcelado, a quantidade de parcelas deve ser informada e maior que zero.", [nameof(QuantParcelas)]);
            }
        }
    }

    public class VendaUpdateDTO : IValidatableObject
    {
        // Change from getter-only to read-write property
        public int Id { get; }
        
        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal? Desconto { get; set; }

        [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
        public string? FormaPag { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de parcelas não pode ser negativa")]
        public int? QuantParcelas { get; set; }

        public int? FuncionarioId { get; set; }

        public int? ClienteId { get; set; }

        // Making ItensVendas explicitly optional with nullable annotation
        public List<ItensVendaCreateDTO>? ItensVendas { get; set; } = null;

        // Update flag to check if items should be updated
        [JsonIgnore]
        public bool UpdateItems => ItensVendas != null;

        [JsonIgnore]
        public decimal ValorTotal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            
            // Skip venda existence check in validation
            // We'll rely on the controller to check this since it's using FirstOrDefaultAsync
            
            // Validar se o funcionário existe (se informado)
            if (FuncionarioId.HasValue && !context.Funcionarios.Any(f => f.Id == FuncionarioId))
                yield return new ValidationResult("Funcionário não encontrado.", [nameof(FuncionarioId)]);

            // Validar se o cliente existe (se informado)
            if (ClienteId.HasValue && !context.Clientes.Any(c => c.Id == ClienteId))
                yield return new ValidationResult("Cliente não encontrado.", [nameof(ClienteId)]);

            // Get venda directly without validation check
            var venda = context.Vendas
                .Include(v => v.ItensVendas)
                .FirstOrDefault(v => v.Id == Id);
                
            // Validate ItensVendas only if they are provided and venda exists
            if (ItensVendas != null && ItensVendas.Any() && venda != null)
            {
                // Validar se as quantidades são positivas
                foreach (var item in ItensVendas)
                {
                    if (item.Quantidade <= 0)
                        yield return new ValidationResult($"A quantidade do item deve ser maior que zero.", [nameof(ItensVendas)]);
                }

                // Pegar todos os IDs de produtos para validação em lote
                var produtoIds = ItensVendas.Select(i => i.ProdutoId).ToList();
                
                // Buscar produtos existentes no banco
                var produtos = context.Produtos
                    .Where(p => produtoIds.Contains(p.Id))
                    .ToDictionary(p => p.Id, p => p);
                
                // Verificar quais produtos não foram encontrados
                var produtosNaoEncontrados = produtoIds
                    .Where(id => !produtos.ContainsKey(id))
                    .ToList();

                // Reportar produtos não encontrados
                if (produtosNaoEncontrados.Any())
                    yield return new ValidationResult(
                        $"Os seguintes produtos não foram encontrados: {string.Join(", ", produtosNaoEncontrados)}", 
                        [nameof(ItensVendas)]);

                // Verificar estoque e calcular o valor total
                ValorTotal = 0;
                foreach (var item in ItensVendas)
                {
                    if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                        continue; // Produto não encontrado, já reportado acima
                        
                    // Verificar estoque (considerando o estoque atual + quantidade já vendida anteriormente nesta venda, se for o mesmo produto)
                    var itemExistente = venda.ItensVendas.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);
                    int quantidadeAtual = itemExistente != null ? itemExistente.Quantidade : 0;
                    
                    if (produto.QuantidadeEstoque + quantidadeAtual < item.Quantidade)
                        yield return new ValidationResult(
                            $"Produto {produto.Descricao} (ID: {produto.Id}) possui apenas {produto.QuantidadeEstoque + quantidadeAtual} unidades em estoque disponíveis, mas foram solicitadas {item.Quantidade}.", 
                            [nameof(ItensVendas)]);
                    
                    // Atribuir o valor correto do produto e calcular o subtotal
                    item.Valor = produto.Valor;
                    ValorTotal += item.Quantidade * produto.Valor;
                }

                // Aplicar desconto se houver
                if (Desconto.HasValue)
                {
                    if (Desconto > ValorTotal)
                        yield return new ValidationResult("O desconto não pode ser maior que o valor total da venda.", [nameof(Desconto)]);
                    else
                        ValorTotal -= Desconto.Value;
                }
            }
            else if (venda != null)
            {
                // If items not provided, calculate total based on existing items
                // Load items if not loaded
                if (!context.Entry(venda).Collection(v => v.ItensVendas).IsLoaded)
                    context.Entry(venda).Collection(v => v.ItensVendas).Load();

                ValorTotal = venda.ItensVendas.Sum(i => i.Quantidade * (decimal)i.Valor);
                
                // Apply new discount if provided, otherwise use existing discount
                if (Desconto.HasValue)
                {
                    if (Desconto > ValorTotal)
                        yield return new ValidationResult("O desconto não pode ser maior que o valor total da venda.", [nameof(Desconto)]);
                    else
                        ValorTotal -= Desconto.Value;
                }
                else if (venda.Desconto.HasValue)
                {
                    ValorTotal -= venda.Desconto.Value;
                }
            }

            // Validar forma de pagamento e parcelas
            if (!string.IsNullOrEmpty(FormaPag) && (FormaPag.ToLower() == "parcelado" || FormaPag.ToLower() == "crediário"))
            {
                if (!QuantParcelas.HasValue || QuantParcelas <= 0)
                    yield return new ValidationResult("Para pagamento parcelado, a quantidade de parcelas deve ser informada e maior que zero.", [nameof(QuantParcelas)]);
            }
        }
    }

    public class ItensVendaDTO
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public int ProdutoId { get; set; }
        public ProdutoDTO Produto { get; set; }
        public int VendaId { get; set; }
        
        // Propriedade calculada para mostrar o subtotal
        public decimal Subtotal => Quantidade * Valor;
    }

    public class ItensVendaCreateDTO
    {
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O produto é obrigatório")]
        public int ProdutoId { get; set; }
        
        [JsonIgnore]
        public decimal Valor { get; set; }
    }
}