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
    public class RecebimentoController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecebimentoDTO>>> GetRecebimentos()
        {
            var recebimentos = await _context.Recebimentos
                .Include(r => r.Caixa)
                .Include(r => r.Venda)
                .Include(r => r.Funcionario)
                .ToListAsync();

            return Ok(_mapper.Map<List<RecebimentoDTO>>(recebimentos));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecebimentoDTO>> GetRecebimento(int id)
        {
            var recebimento = await _context.Recebimentos
                .Include(r => r.Caixa)
                .Include(r => r.Venda)
                .Include(r => r.Funcionario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recebimento == null)
                return NotFound($"Recebimento com ID {id} não encontrado.");

            return _mapper.Map<RecebimentoDTO>(recebimento);
        }

        [HttpPost]
        public async Task<ActionResult<RecebimentoDTO>> PostRecebimento(RecebimentoDTO recebimentoDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verifica se existe um caixa aberto
                var caixa = await _context.Caixas
                    .FirstOrDefaultAsync(c => c.Id == recebimentoDto.CaixaId && c.Status);
                
                if (caixa == null)
                    return BadRequest("Não há caixa aberto para registrar o recebimento.");

                // Verifica a venda
                var venda = await _context.Vendas
                    .Include(v => v.Recebimentos)
                    .FirstOrDefaultAsync(v => v.Id == recebimentoDto.VendaId);

                if (venda == null)
                    return BadRequest("Venda não encontrada.");

                // Calcula valor pendente
                var valorPendente = venda.Valor - venda.Recebimentos.Sum(r => r.Valor);
                
                if (recebimentoDto.Valor > valorPendente)
                    return BadRequest($"Valor do recebimento (R$ {recebimentoDto.Valor}) maior que o valor pendente (R$ {valorPendente}).");

                // Verifica funcionário
                var funcionarioExists = await _context.Funcionarios
                    .AnyAsync(f => f.Id == recebimentoDto.FuncionarioId);

                if (!funcionarioExists)
                    return BadRequest("Funcionário não encontrado.");

                var recebimento = _mapper.Map<Recebimento>(recebimentoDto);
                recebimento.Data = DateTime.Now;

                _context.Recebimentos.Add(recebimento);

                // Atualiza valores do caixa
                caixa.ValorCreditos += recebimento.Valor;
                caixa.CalcularSaldoFinal();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(
                    nameof(GetRecebimento), 
                    new { id = recebimento.Id }, 
                    _mapper.Map<RecebimentoDTO>(recebimento)
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao registrar recebimento: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecebimento(int id, RecebimentoDTO recebimentoDto)
        {
            if (id != recebimentoDto.Id)
                return BadRequest("O ID da URL não corresponde ao ID do objeto.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = await _context.Recebimentos
                    .Include(r => r.Caixa)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recebimento == null)
                    return NotFound($"Recebimento com ID {id} não encontrado.");

                if (!recebimento.Caixa.Status)
                    return BadRequest("Não é possível alterar um recebimento de um caixa fechado.");

                var valorAntigo = recebimento.Valor;
                _mapper.Map(recebimentoDto, recebimento);

                // Atualiza valores do caixa
                recebimento.Caixa.ValorCreditos = recebimento.Caixa.ValorCreditos - valorAntigo + recebimento.Valor;
                recebimento.Caixa.CalcularSaldoFinal();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(_mapper.Map<RecebimentoDTO>(recebimento));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao atualizar recebimento: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecebimento(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = await _context.Recebimentos
                    .Include(r => r.Caixa)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recebimento == null)
                    return NotFound($"Recebimento com ID {id} não encontrado.");

                if (!recebimento.Caixa.Status)
                    return BadRequest("Não é possível excluir um recebimento de um caixa fechado.");

                // Atualiza valores do caixa
                recebimento.Caixa.ValorCreditos -= recebimento.Valor;
                recebimento.Caixa.CalcularSaldoFinal();

                _context.Recebimentos.Remove(recebimento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao excluir recebimento: {ex.Message}");
            }
        }
    }
}