using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace GestaoVendasWeb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public VendaController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> GetVendas()
        {
            var vendas = await _context.Vendas
                .Include(v => v.ItensVendas)
                    .ThenInclude(iv => iv.Produto)
                .Include(v => v.Funcionario)
                .Include(v => v.Cliente)
                .Include(v => v.Recebimentos)
                .ToListAsync();

            return Ok(_mapper.Map<List<VendaDTO>>(vendas));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendaDTO>> GetVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.ItensVendas)
                    .ThenInclude(iv => iv.Produto)
                .Include(v => v.Funcionario)
                .Include(v => v.Cliente)
                .Include(v => v.Recebimentos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                return NotFound($"Venda com ID {id} não encontrada.");
            }

            return _mapper.Map<VendaDTO>(venda);
        }

        [HttpPost]
        public async Task<ActionResult<VendaDTO>> PostVenda(VendaCreateDTO vendaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = _mapper.Map<Venda>(vendaDto);
                
                foreach (var item in venda.ItensVendas)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.QuantidadeEstoque -= item.Quantidade;
                        item.Valor = (double)produto.Valor;
                    }
                }

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();
                
                var vendaCompleta = await _context.Vendas
                    .Include(v => v.ItensVendas)
                        .ThenInclude(iv => iv.Produto)
                    .Include(v => v.Funcionario)
                    .Include(v => v.Cliente)
                    .Include(v => v.Recebimentos)
                    .FirstOrDefaultAsync(v => v.Id == venda.Id);
                    
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetVenda), new { id = venda.Id }, _mapper.Map<VendaDTO>(vendaCompleta));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao criar venda: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchVenda(int id, VendaUpdateDTO vendaDto)
        {   
            try
            {               
                var venda = await _context.Vendas
                    .Include(v => v.ItensVendas)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound($"Venda com ID {id} não encontrada.");
                }
                
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (vendaDto.UpdateItems)
                    {
                        foreach (var item in venda.ItensVendas)
                        {
                            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                            if (produto != null)
                                produto.QuantidadeEstoque += item.Quantidade;
                        }

                        _context.ItensVendas.RemoveRange(venda.ItensVendas);
                        
                        venda.ItensVendas.Clear();
                        foreach (var itemDto in vendaDto.ItensVendas)
                        {
                            var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);
                            if (produto != null)
                            {
                                var novoItem = new ItensVenda
                                {
                                    ProdutoId = itemDto.ProdutoId,
                                    Quantidade = itemDto.Quantidade,
                                    Valor = (double)produto.Valor,
                                    VendaId = venda.Id
                                };
                                venda.ItensVendas.Add(novoItem);
                                
                                produto.QuantidadeEstoque -= itemDto.Quantidade;
                            }
                        }
                        
                        venda.CalcularValorTotal();
                    }

                    if (vendaDto.Desconto.HasValue)
                    {
                        venda.Desconto = vendaDto.Desconto;
                        if (!vendaDto.UpdateItems)
                            venda.CalcularValorTotal();
                    }
                        
                    if (!string.IsNullOrEmpty(vendaDto.FormaPag))
                        venda.FormaPag = vendaDto.FormaPag;
                        
                    if (vendaDto.QuantParcelas.HasValue)
                        venda.QuantParcelas = vendaDto.QuantParcelas;
                        
                    if (vendaDto.FuncionarioId.HasValue)
                        venda.FuncionarioId = vendaDto.FuncionarioId.Value;
                        
                    if (vendaDto.ClienteId.HasValue)
                        venda.ClienteId = vendaDto.ClienteId.Value;

                    await _context.SaveChangesAsync();
                    
                    var vendaCompleta = await _context.Vendas
                        .Include(v => v.ItensVendas)
                            .ThenInclude(iv => iv.Produto)
                        .Include(v => v.Funcionario)
                        .Include(v => v.Cliente)
                        .Include(v => v.Recebimentos)
                        .FirstOrDefaultAsync(v => v.Id == venda.Id);
                        
                    await transaction.CommitAsync();

                    return Ok(_mapper.Map<VendaDTO>(vendaCompleta));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Erro ao atualizar venda: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar requisição: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = await _context.Vendas
                    .Include(v => v.ItensVendas)
                    .Include(v => v.Recebimentos)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound($"Venda com ID {id} não encontrada.");
                }
                
                if (venda.Recebimentos.Any())
                {
                    return BadRequest("Não é possível excluir uma venda que já possui recebimentos registrados.");
                }

                foreach (var item in venda.ItensVendas)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                        produto.QuantidadeEstoque += item.Quantidade;
                }

                _context.ItensVendas.RemoveRange(venda.ItensVendas);
                _context.Vendas.Remove(venda);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao excluir venda: {ex.Message}");
            }
        }
    }
}