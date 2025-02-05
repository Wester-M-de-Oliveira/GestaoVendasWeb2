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
                .Include(v => v.Funcionario)
                .Include(v => v.Cliente)
                .ToListAsync();

            return Ok(_mapper.Map<List<VendaDTO>>(vendas));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendaDTO>> GetVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.ItensVendas)
                .Include(v => v.Funcionario)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                return NotFound($"Venda com ID {id} não encontrada.");
            }

            return _mapper.Map<VendaDTO>(venda);
        }

        [HttpPost]
        public async Task<ActionResult<VendaDTO>> PostVenda(VendaDTO vendaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar relacionamentos
                var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == vendaDto.FuncionarioId);
                var clienteExists = await _context.Clientes.AnyAsync(c => c.Id == vendaDto.ClienteId);

                if (!funcionarioExists || !clienteExists)
                {
                    return BadRequest("Funcionário ou Cliente não encontrado.");
                }

                // Validar produtos
                foreach (var item in vendaDto.ItensVendas)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto == null)
                    {
                        return BadRequest($"Produto com ID {item.ProdutoId} não encontrado.");
                    }
                }

                var venda = _mapper.Map<Venda>(vendaDto);
                venda.DataVenda = DateTime.Now;
                
                // Calcula valor total com desconto
                if (vendaDto.Desconto.HasValue)
                {
                    venda.Valor = (decimal)(vendaDto.ItensVendas.Sum(i => i.Valor * i.Quantidade) - vendaDto.Desconto.Value);
                }

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetVenda), new { id = venda.Id }, _mapper.Map<VendaDTO>(venda));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao criar venda: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenda(int id, VendaDTO vendaDto)
        {
            if (id != vendaDto.Id)
            {
                return BadRequest("O ID da URL não corresponde ao ID do objeto.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = await _context.Vendas
                    .Include(v => v.ItensVendas)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound($"Venda com ID {id} não encontrada.");
                }

                // Validar relacionamentos
                var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == vendaDto.FuncionarioId);
                var clienteExists = await _context.Clientes.AnyAsync(c => c.Id == vendaDto.ClienteId);

                if (!funcionarioExists || !clienteExists)
                {
                    return BadRequest("Funcionário ou Cliente não encontrado.");
                }

                // Remove itens existentes
                _context.ItensVendas.RemoveRange(venda.ItensVendas);

                // Mapeia novas propriedades
                _mapper.Map(vendaDto, venda);

                // Recalcula valor com desconto
                if (vendaDto.Desconto.HasValue)
                {
                    venda.Valor = (decimal)(vendaDto.ItensVendas.Sum(i => i.Valor * i.Quantidade) - vendaDto.Desconto.Value);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(_mapper.Map<VendaDTO>(venda));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao atualizar venda: {ex.Message}");
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
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound($"Venda com ID {id} não encontrada.");
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

        private async Task<bool> VendaExists(int id)
        {
            return await _context.Vendas.AnyAsync(v => v.Id == id);
        }
    }
}