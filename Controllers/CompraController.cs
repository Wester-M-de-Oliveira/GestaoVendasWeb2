using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace GestaoVendasWeb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompraController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras()
        {
            try
            {
                var compras = await _context.Compras
                    .Include(c => c.ItensCompras)
                    .Include(c => c.Funcionario)
                    .Include(c => c.Fornecedor)
                    .Include(c => c.Pagamentos)
                    .AsNoTracking()
                    .ToListAsync();

                var comprasDto = _mapper.Map<List<CompraDTO>>(compras);

                return Ok(comprasDto);
            }
            catch (Exception e)
            {
                return Problem(e.Message, e.Source);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            try
            {
                var compra = await _context.Compras
                    .Include(c => c.ItensCompras)
                        .ThenInclude(i => i.Produto)
                    .Include(c => c.Funcionario)
                    .Include(c => c.Fornecedor)
                    .Include(c => c.Pagamentos)
                    .FirstOrDefaultAsync(c => c.Id == id);


                if (compra == null)
                {
                    return NotFound();
                }
                return _mapper.Map<CompraDTO>(compra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar compra: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CompraDTO>> PostCompra(CompraCreateDto compraDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (compraDto.ItensCompra == null || !compraDto.ItensCompra.Any())
                    return BadRequest("A compra deve ter pelo menos um item.");

                var compra = _mapper.Map<Compra>(compraDto);

                compra.Valor = compraDto.Valor;

                foreach (var item in compra.ItensCompras)
                {
                    item.Compra = compra;
                    item.CompraId = compra.Id;
                }

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetCompra), new { id = compra.Id }, _mapper.Map<CompraDTO>(compra));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao processar a compra: {ex.Message}");
            }
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCompra(int id, [FromBody] CompraUpdateDto compraDto)
        {
            var compra = await _context.Compras
                .Include(c => c.ItensCompras)
                .Include(c => c.Funcionario)
                .Include(c => c.Fornecedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
                return NotFound($"Compra com ID {id} não encontrada.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (compraDto.Data.HasValue)
                    compra.Data = compraDto.Data.Value;

                if (!string.IsNullOrWhiteSpace(compraDto.FormaPag))
                    compra.FormaPag = compraDto.FormaPag;

                if (compraDto.Status.HasValue)
                    compra.Status = compraDto.Status.Value;

                if (!string.IsNullOrWhiteSpace(compraDto.Observacao))
                    compra.Observacao = compraDto.Observacao;

                if (compraDto.FuncionarioId.HasValue)
                {
                    var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == compraDto.FuncionarioId);
                    if (!funcionarioExists)
                        return BadRequest("Funcionário não encontrado.");

                    compra.FuncionarioId = compraDto.FuncionarioId.Value;
                }

                if (compraDto.FornecedorId.HasValue)
                {
                    var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == compraDto.FornecedorId);
                    if (!fornecedorExists)
                        return BadRequest("Fornecedor não encontrado.");

                    compra.FornecedorId = compraDto.FornecedorId.Value;
                }

                if (compraDto.ItensCompra != null && compraDto.ItensCompra.Any())
                {
                    _context.ItensCompras.RemoveRange(compra.ItensCompras);

                    var novosItens = compraDto.ItensCompra.Select(item => new ItensCompra
                    {
                        CompraId = compra.Id,
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade
                    }).ToList();

                    await _context.ItensCompras.AddRangeAsync(novosItens);

                    var produtos = await _context.Produtos
                        .Where(p => compraDto.ItensCompra.Select(i => i.ProdutoId).Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id, p => p.PrecoCompra);

                    compra.Valor = compraDto.ItensCompra.Sum(item =>
                        produtos.TryGetValue(item.ProdutoId, out var preco) ? item.Quantidade * preco : 0);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var compraAtualizada = await _context.Compras
                    .Include(c => c.ItensCompras)
                    .Include(c => c.Funcionario)
                    .Include(c => c.Fornecedor)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return Ok(_mapper.Map<CompraDTO>(compraAtualizada));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var compra = await _context.Compras
                    .Include(c => c.ItensCompras)
                    .Include(c => c.Pagamentos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (compra == null)
                {
                    return NotFound($"Compra com ID {id} não encontrada.");
                }

                if (compra.Status == StatusCompra.Finalizada && compra.Pagamentos.Any())
                {
                    return BadRequest("Não é possível excluir uma compra finalizada com pagamentos.");
                }

                _context.ItensCompras.RemoveRange(compra.ItensCompras);
                _context.Pagamentos.RemoveRange(compra.Pagamentos);
                _context.Compras.Remove(compra);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao excluir compra: {ex.Message}");
            }
        }

        private async Task<bool> CompraExists(int id)
        {
            return await _context.Compras.AnyAsync(c => c.Id == id);
        }
    }
}