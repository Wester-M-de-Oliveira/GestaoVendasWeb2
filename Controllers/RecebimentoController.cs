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
                    .ThenInclude(v => v.Recebimentos)
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
                    .ThenInclude(v => v.Recebimentos)
                .Include(r => r.Funcionario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recebimento == null)
                return NotFound($"Recebimento com ID {id} não encontrado.");

            return _mapper.Map<RecebimentoDTO>(recebimento);
        }

        [HttpPost]
        public async Task<ActionResult<RecebimentoCreateDTO>> PostRecebimento(RecebimentoCreateDTO recebimentoDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = _mapper.Map<Recebimento>(recebimentoDto);

                _context.Recebimentos.Add(recebimento);

                // Atualiza valores do caixa
                var caixa = await _context.Caixas.FindAsync(recebimentoDto.CaixaId);
                caixa.ValorCreditos += recebimento.Valor;
                caixa.CalcularSaldoFinal();

                await _context.SaveChangesAsync();
                
                // Recarregar o recebimento com seus relacionamentos
                await _context.Entry(recebimento).Reference(r => r.Caixa).LoadAsync();
                await _context.Entry(recebimento).Reference(r => r.Funcionario).LoadAsync();
                
                // Carregar a venda com seus recebimentos para calcular o valor restante
                await _context.Entry(recebimento).Reference(r => r.Venda).LoadAsync();
                await _context.Entry(recebimento.Venda).Collection(v => v.Recebimentos).LoadAsync();
                
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
                // Improve error message with inner exception details
                string errorDetails = ex.InnerException != null ? $" - Inner: {ex.InnerException.Message}" : "";
                return StatusCode(500, $"Erro ao registrar recebimento: {ex.Message}{errorDetails}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchRecebimento(int id, RecebimentoUpdateDTO recebimentoDto)
        {
            recebimentoDto.Id = id; // Garantir que o ID está definido para validação

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = await _context.Recebimentos
                    .Include(r => r.Caixa)
                    .Include(r => r.Venda)
                        .ThenInclude(v => v.Recebimentos)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recebimento == null)
                    return NotFound($"Recebimento com ID {id} não encontrado.");

                if (!recebimento.Caixa.Status)
                    return BadRequest("Não é possível alterar um recebimento de um caixa fechado.");

                // Valores originais para atualização do caixa
                var valorAntigo = recebimento.Valor;
                var caixaAntigo = recebimento.CaixaId;

                // Aplicar apenas as alterações fornecidas
                _mapper.Map(recebimentoDto, recebimento);

                // Se o valor foi alterado ou o caixa foi alterado, atualizar os saldos
                if (recebimentoDto.Valor.HasValue || recebimentoDto.CaixaId.HasValue)
                {
                    // Se mudou o caixa
                    if (recebimentoDto.CaixaId.HasValue && recebimentoDto.CaixaId.Value != caixaAntigo)
                    {
                        // Atualizar saldo do caixa antigo (remover o valor)
                        var caixaAntigoObj = await _context.Caixas.FindAsync(caixaAntigo);
                        caixaAntigoObj.ValorCreditos -= valorAntigo;
                        caixaAntigoObj.CalcularSaldoFinal();

                        // Atualizar saldo do novo caixa (adicionar o valor)
                        var novoCaixa = await _context.Caixas.FindAsync(recebimentoDto.CaixaId.Value);
                        novoCaixa.ValorCreditos += recebimento.Valor;
                        novoCaixa.CalcularSaldoFinal();
                    }
                    // Se apenas o valor mudou
                    else if (recebimentoDto.Valor.HasValue)
                    {
                        // Atualizar saldo do mesmo caixa com a diferença
                        recebimento.Caixa.ValorCreditos = recebimento.Caixa.ValorCreditos - valorAntigo + recebimento.Valor;
                        recebimento.Caixa.CalcularSaldoFinal();
                    }
                }

                await _context.SaveChangesAsync();
                
                // Recarregar o recebimento para a resposta
                await _context.Entry(recebimento).Reference(r => r.Funcionario).LoadAsync();
                
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