using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace GestaoVendasWeb2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PagamentoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PagamentoController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PagamentoDTO>>> GetPagamentos()
    {
        var pagamentos = await _context.Pagamentos
            .Include(p => p.Caixa)
            .Include(p => p.Compra)
            .Include(p => p.Despesa)
            .Include(p => p.Funcionario)
            .ToListAsync();

        return Ok(_mapper.Map<List<PagamentoDTO>>(pagamentos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PagamentoDTO>> GetPagamento(int id)
    {
        var pagamento = await _context.Pagamentos
            .Include(p => p.Caixa)
            .Include(p => p.Compra)
            .Include(p => p.Despesa)
            .Include(p => p.Funcionario)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pagamento == null)
            return NotFound($"Pagamento com ID {id} não encontrado.");

        return _mapper.Map<PagamentoDTO>(pagamento);
    }

    [HttpPost]
    public async Task<ActionResult<PagamentoDTO>> PostPagamento(PagamentoCreateDTO pagamentoDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var pagamento = _mapper.Map<Pagamento>(pagamentoDto);
            
            _context.Pagamentos.Add(pagamento);
            
            var caixa = await _context.Caixas.FindAsync(pagamentoDto.CaixaId);
            caixa.ValorDebitos += pagamento.Valor;
            caixa.CalcularSaldoFinal();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _context.Entry(pagamento)
                .Reference(p => p.Caixa).LoadAsync();
            
            if (pagamento.CompraId > 0)
                await _context.Entry(pagamento)
                    .Reference(p => p.Compra).LoadAsync();
                    
            if (pagamento.DespesaId > 0)
                await _context.Entry(pagamento)
                    .Reference(p => p.Despesa).LoadAsync();
                    
            await _context.Entry(pagamento)
                .Reference(p => p.Funcionario).LoadAsync();

            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.Id }, 
                _mapper.Map<PagamentoDTO>(pagamento));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao registrar pagamento: {ex.Message}");
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchPagamento(int id, PagamentoUpdateDTO pagamentoDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var pagamento = await _context.Pagamentos
                .Include(p => p.Caixa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null)
                return NotFound($"Pagamento com ID {id} não encontrado.");

            if (!pagamento.Caixa.Status)
                return BadRequest("Não é possível alterar um pagamento de um caixa fechado.");

            var valorAntigo = pagamento.Valor;
            var caixaAntigo = pagamento.CaixaId;

            if (pagamentoDto.Valor.HasValue)
                pagamento.Valor = pagamentoDto.Valor.Value;
                
            if (!string.IsNullOrEmpty(pagamentoDto.FormaPag))
                pagamento.FormaPag = pagamentoDto.FormaPag;
                
            if (pagamentoDto.FuncionarioId.HasValue)
            {
                var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == pagamentoDto.FuncionarioId);
                if (!funcionarioExists)
                    return BadRequest($"Funcionário com ID {pagamentoDto.FuncionarioId} não encontrado.");
                    
                pagamento.FuncionarioId = pagamentoDto.FuncionarioId.Value;
            }
            
            if (pagamentoDto.CompraId.HasValue)
            {
                if (pagamentoDto.CompraId.Value > 0)
                {
                    var compraExists = await _context.Compras.AnyAsync(c => c.Id == pagamentoDto.CompraId);
                    if (!compraExists)
                        return BadRequest($"Compra com ID {pagamentoDto.CompraId} não encontrada.");
                }
                
                pagamento.CompraId = pagamentoDto.CompraId.Value > 0 ? pagamentoDto.CompraId : null;
            }
            
            if (pagamentoDto.DespesaId.HasValue)
            {
                if (pagamentoDto.DespesaId.Value > 0)
                {
                    var despesaExists = await _context.Despesas.AnyAsync(d => d.Id == pagamentoDto.DespesaId);
                    if (!despesaExists)
                        return BadRequest($"Despesa com ID {pagamentoDto.DespesaId} não encontrada.");
                }
                
                pagamento.DespesaId = pagamentoDto.DespesaId.Value > 0 ? pagamentoDto.DespesaId : null;
            }

            if (!pagamento.CompraId.HasValue && !pagamento.DespesaId.HasValue)
                return BadRequest("É necessário informar uma Compra ou Despesa para o pagamento.");
            
            if (pagamentoDto.CaixaId.HasValue)
            {
                var novoCaixa = await _context.Caixas.FindAsync(pagamentoDto.CaixaId.Value);
                if (novoCaixa == null)
                    return BadRequest($"Caixa com ID {pagamentoDto.CaixaId} não encontrado.");
                    
                if (!novoCaixa.Status)
                    return BadRequest("Não é possível associar um pagamento a um caixa fechado.");
                    
                if (pagamentoDto.CaixaId.Value != caixaAntigo)
                {
                    var caixaAntigoObj = await _context.Caixas.FindAsync(caixaAntigo);
                    caixaAntigoObj.ValorDebitos -= valorAntigo;
                    caixaAntigoObj.CalcularSaldoFinal();
                    
                    novoCaixa.ValorDebitos += pagamento.Valor;
                    novoCaixa.CalcularSaldoFinal();
                    
                    pagamento.CaixaId = pagamentoDto.CaixaId.Value;
                    pagamento.Caixa = novoCaixa;
                }
            }
            else if (pagamentoDto.Valor.HasValue)
            {
                pagamento.Caixa.ValorDebitos = pagamento.Caixa.ValorDebitos - valorAntigo + pagamento.Valor;
                pagamento.Caixa.CalcularSaldoFinal();
            }

            _context.Entry(pagamento).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var pagamentoAtualizado = await _context.Pagamentos
                .Include(p => p.Caixa)
                .Include(p => p.Funcionario)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (pagamentoAtualizado.CompraId.HasValue)
                await _context.Entry(pagamentoAtualizado).Reference(p => p.Compra).LoadAsync();
                
            if (pagamentoAtualizado.DespesaId.HasValue)
                await _context.Entry(pagamentoAtualizado).Reference(p => p.Despesa).LoadAsync();

            return Ok(_mapper.Map<PagamentoDTO>(pagamentoAtualizado));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao atualizar pagamento: {ex.Message} - {ex.InnerException?.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePagamento(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var pagamento = await _context.Pagamentos
                .Include(p => p.Caixa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null)
                return NotFound($"Pagamento com ID {id} não encontrado.");

            if (!pagamento.Caixa.Status)
                return BadRequest("Não é possível excluir um pagamento de um caixa fechado.");

            pagamento.Caixa.ValorDebitos -= pagamento.Valor;
            pagamento.Caixa.CalcularSaldoFinal();

            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao excluir pagamento: {ex.Message}");
        }
    }
}