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
    public class RecebimentoController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obter todos os recebimentos
        /// </summary>
        /// <returns>Lista de recebimentos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RecebimentoDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        /// <summary>
        /// Obter recebimento por ID
        /// </summary>
        /// <param name="id">ID do recebimento</param>
        /// <returns>Detalhes do recebimento</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecebimentoDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Registrar novo recebimento
        /// </summary>
        /// <param name="recebimentoDto">Dados do recebimento</param>
        /// <returns>Recebimento criado</returns>
        [HttpPost]
        [Authorize(Roles = "admin,caixa,gerente")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RecebimentoDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecebimentoCreateDTO>> PostRecebimento(RecebimentoCreateDTO recebimentoDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = _mapper.Map<Recebimento>(recebimentoDto);

                _context.Recebimentos.Add(recebimento);

                var caixa = await _context.Caixas.FindAsync(recebimentoDto.CaixaId);
                caixa.ValorCreditos += recebimento.Valor;
                caixa.CalcularSaldoFinal();

                await _context.SaveChangesAsync();
                
                await _context.Entry(recebimento).Reference(r => r.Caixa).LoadAsync();
                await _context.Entry(recebimento).Reference(r => r.Funcionario).LoadAsync();
                
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
                string errorDetails = ex.InnerException != null ? $" - Inner: {ex.InnerException.Message}" : "";
                return StatusCode(500, $"Erro ao registrar recebimento: {ex.Message}{errorDetails}");
            }
        }

        /// <summary>
        /// Atualizar recebimento
        /// </summary>
        /// <param name="id">ID do recebimento</param>
        /// <param name="recebimentoDto">Dados atualizados</param>
        /// <returns>Recebimento atualizado</returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "admin,gerente")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecebimentoDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchRecebimento(int id, RecebimentoUpdateDTO recebimentoDto)
        {
            recebimentoDto.Id = id;

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

                var valorAntigo = recebimento.Valor;
                var caixaAntigo = recebimento.CaixaId;

                _mapper.Map(recebimentoDto, recebimento);

                if (recebimentoDto.Valor.HasValue || recebimentoDto.CaixaId.HasValue)
                {
                    if (recebimentoDto.CaixaId.HasValue && recebimentoDto.CaixaId.Value != caixaAntigo)
                    {
                        var caixaAntigoObj = await _context.Caixas.FindAsync(caixaAntigo);
                        caixaAntigoObj.ValorCreditos -= valorAntigo;
                        caixaAntigoObj.CalcularSaldoFinal();

                        var novoCaixa = await _context.Caixas.FindAsync(recebimentoDto.CaixaId.Value);
                        novoCaixa.ValorCreditos += recebimento.Valor;
                        novoCaixa.CalcularSaldoFinal();
                    }
                    else if (recebimentoDto.Valor.HasValue)
                    {
                        recebimento.Caixa.ValorCreditos = recebimento.Caixa.ValorCreditos - valorAntigo + recebimento.Valor;
                        recebimento.Caixa.CalcularSaldoFinal();
                    }
                }

                await _context.SaveChangesAsync();
                
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

        /// <summary>
        /// Excluir recebimento
        /// </summary>
        /// <param name="id">ID do recebimento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        
        /// <summary>
        /// Obter recebimentos por venda
        /// </summary>
        /// <param name="vendaId">ID da venda</param>
        /// <returns>Lista de recebimentos da venda</returns>
        [HttpGet("venda/{vendaId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RecebimentoDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RecebimentoDTO>>> GetRecebimentosByVenda(int vendaId)
        {
            var venda = await _context.Vendas
                .Include(v => v.Recebimentos)
                    .ThenInclude(r => r.Caixa)
                .Include(v => v.Recebimentos)
                    .ThenInclude(r => r.Funcionario)
                .FirstOrDefaultAsync(v => v.Id == vendaId);

            if (venda == null)
                return NotFound($"Venda com ID {vendaId} não encontrada.");

            var recebimentos = venda.Recebimentos.ToList();
            return Ok(_mapper.Map<List<RecebimentoDTO>>(recebimentos));
        }
    }
}