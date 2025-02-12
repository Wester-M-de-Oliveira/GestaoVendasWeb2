using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using AutoMapper;

namespace GestaoVendasWeb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras()
        {
            var compras = await _context.Compras
                .Include(c => c.ItensCompras)
                .Include(c => c.Funcionario)
                .Include(c => c.Fornecedor)
                .Include(c => c.Pagamentos)
                .Select(c => new CompraDTO
                {
                    Id = c.Id,
                    Data = c.Data,
                    Valor = c.Valor,
                    FormaPag = c.FormaPag,
                    Status = c.Status,
                    Observacao = c.Observacao,
                    FuncionarioId = c.FuncionarioId,
                    FornecedorId = c.FornecedorId,
                    Funcionario = new FuncionarioDTO { Id = c.Funcionario.Id, Nome = c.Funcionario.Nome },
                    Fornecedor = new FornecedorDTO { Id = c.Fornecedor.Id, NomeFantasia = c.Fornecedor.NomeFantasia },
                    ItensCompra = c.ItensCompras.Select(i => new ItensCompraDTO
                    {
                        Id = i.Id,
                        Quantidade = i.Quantidade,
                        Valor = i.Valor,
                        ProdutoId = i.ProdutoId,
                        CompraId = i.CompraId
                    }).ToList(),
                    Pagamentos = c.Pagamentos.Select(p => new PagamentoDTO
                    {
                        Id = p.Id,
                        Valor = (double)p.Valor,
                        Data = p.Data,
                        FormaPag = p.FormaPag
                    }).ToList()
                })
                .ToListAsync();

            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.ItensCompras)
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

        [HttpPost]
        public async Task<ActionResult<CompraDTO>> PostCompra(CompraDTO compraDto)
        {
            var compra = _mapper.Map<Compra>(compraDto);
            
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompra), new { id = compra.Id }, _mapper.Map<CompraDTO>(compra));
        }

    // PUT: api/Compra/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompra(int id, CompraDTO compraDto)
    {
        try
        {
            if (id != compraDto.Id)
            {
                return BadRequest("O ID da URL não corresponde ao ID do objeto.");
            }

            var compra = await _context.Compras
                .Include(c => c.ItensCompras)
                .Include(c => c.Funcionario)
                .Include(c => c.Fornecedor)
                .Include(c => c.Pagamentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                return NotFound($"Compra com ID {id} não encontrada.");
            }

            // Validar relacionamentos
            var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == compraDto.FuncionarioId);
            var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == compraDto.FornecedorId);

            if (!funcionarioExists || !fornecedorExists)
            {
                return BadRequest("Funcionário ou Fornecedor não encontrado.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove itens existentes
                _context.ItensCompras.RemoveRange(compra.ItensCompras);

                // Mapeia as novas propriedades
                _mapper.Map(compraDto, compra);

                // Recalcula o valor total
                compra.RecalcularValorTotal();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Retorna a compra atualizada
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
        catch (DbUpdateConcurrencyException)
        {
            if (!await CompraExists(id))
            {
                return NotFound($"Compra com ID {id} não encontrada.");
            }
            throw;
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // DELETE: api/Compra/5
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

            // Verifica se a compra pode ser excluída
            if (compra.Status == StatusCompra.Finalizada && compra.Pagamentos.Any())
            {
                return BadRequest("Não é possível excluir uma compra finalizada com pagamentos.");
            }

            // Remove itens relacionados
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