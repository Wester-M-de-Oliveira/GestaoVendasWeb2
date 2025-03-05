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
    public class CaixaController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaixaDTO>>> GetCaixas()
        {
            var caixas = await _context.Caixas
                .Include(c => c.Recebimentos)
                .Include(c => c.Pagamentos)
                .ToListAsync();

            return Ok(_mapper.Map<List<CaixaDTO>>(caixas));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaixaDTO>> GetCaixa(int id)
        {
            var caixa = await _context.Caixas
                .Include(c => c.Recebimentos)
                .Include(c => c.Pagamentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (caixa == null)
                return NotFound($"Caixa com ID {id} não encontrado.");

            return _mapper.Map<CaixaDTO>(caixa);
        }

        [HttpGet("aberto")]
        public async Task<ActionResult<CaixaDTO>> GetCaixaAberto()
        {
            var caixa = await _context.Caixas
                .Include(c => c.Recebimentos)
                .Include(c => c.Pagamentos)
                .FirstOrDefaultAsync(c => c.Status);

            if (caixa == null)
                return NotFound("Não há caixa aberto no momento.");

            return _mapper.Map<CaixaDTO>(caixa);
        }

        [HttpPost]
        public async Task<ActionResult<CaixaCreateUpdateDTO>> AbrirCaixa(CaixaCreateUpdateDTO caixaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verifica se já existe um caixa aberto
                var caixaAberto = await _context.Caixas.AnyAsync(c => c.Status);
                if (caixaAberto)
                    return BadRequest("Já existe um caixa aberto.");

                var caixa = _mapper.Map<Caixa>(caixaDto);
                caixa.DataAbertura = DateTime.Now;
                caixa.Status = true;

                _context.Caixas.Add(caixa);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetCaixa), new { id = caixa.Id }, _mapper.Map<CaixaCreateUpdateDTO>(caixa));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao abrir caixa: {ex.Message}");
            }
        }

        [HttpPut("{id}/fechar")]
        public async Task<IActionResult> FecharCaixa(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var caixa = await _context.Caixas
                    .Include(c => c.Recebimentos)
                    .Include(c => c.Pagamentos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (caixa == null)
                    return NotFound($"Caixa com ID {id} não encontrado.");

                if (!caixa.Status)
                    return BadRequest("Este caixa já está fechado.");

                caixa.Fechar();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(_mapper.Map<CaixaCreateUpdateDTO>(caixa));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao fechar caixa: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCaixa(int id, CaixaCreateUpdateDTO caixaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var caixa = await _context.Caixas.FindAsync(id);
                if (caixa == null)
                    return NotFound($"Caixa com ID {id} não encontrado.");

                if (!caixa.Status)
                    return BadRequest("Não é possível alterar um caixa fechado.");

                _mapper.Map(caixaDto, caixa);
                caixa.CalcularSaldoFinal();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(_mapper.Map<CaixaCreateUpdateDTO>(caixa));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao atualizar caixa: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaixa(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var caixa = await _context.Caixas
                    .Include(c => c.Recebimentos)
                    .Include(c => c.Pagamentos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (caixa == null)
                    return NotFound($"Caixa com ID {id} não encontrado.");

                if (caixa.Recebimentos.Any() || caixa.Pagamentos.Any())
                    return BadRequest("Não é possível excluir um caixa com movimentações.");

                _context.Caixas.Remove(caixa);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erro ao excluir caixa: {ex.Message}");
            }
        }
    }
}