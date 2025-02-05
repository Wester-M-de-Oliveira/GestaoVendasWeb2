using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using AutoMapper;

namespace GestaoVendasWeb2.Controllers;

[Route("api/[controller]")]
[ApiController]
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
    public async Task<ActionResult<PagamentoDTO>> PostPagamento(PagamentoDTO pagamentoDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validar caixa aberto
            var caixa = await _context.Caixas.FindAsync(pagamentoDto.CaixaId);
            if (caixa == null || !caixa.Status)
                return BadRequest("Caixa não encontrado ou fechado.");

            // Validar relacionamentos
            var relacionamentosValidos = await ValidarRelacionamentos(pagamentoDto);
            if (!relacionamentosValidos.IsValid)
                return BadRequest(relacionamentosValidos.Message);

            var pagamento = _mapper.Map<Pagamento>(pagamentoDto);
            pagamento.Data = DateTime.Now;

            _context.Pagamentos.Add(pagamento);
            
            // Atualizar saldo do caixa
            caixa.ValorDebitos += pagamento.Valor;
            caixa.CalcularSaldoFinal();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.Id }, 
                _mapper.Map<PagamentoDTO>(pagamento));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao registrar pagamento: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPagamento(int id, PagamentoDTO pagamentoDto)
    {
        if (id != pagamentoDto.Id)
            return BadRequest("O ID da URL não corresponde ao ID do objeto.");

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
            _mapper.Map(pagamentoDto, pagamento);

            // Atualizar saldo do caixa
            pagamento.Caixa.ValorDebitos = pagamento.Caixa.ValorDebitos - valorAntigo + pagamento.Valor;
            pagamento.Caixa.CalcularSaldoFinal();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(_mapper.Map<PagamentoDTO>(pagamento));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao atualizar pagamento: {ex.Message}");
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

            // Atualizar saldo do caixa
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

    private async Task<(bool IsValid, string Message)> ValidarRelacionamentos(PagamentoDTO dto)
    {
        var funcionarioExists = await _context.Funcionarios.AnyAsync(f => f.Id == dto.FuncionarioId);
        if (!funcionarioExists)
            return (false, "Funcionário não encontrado.");

        var compraExists = await _context.Compras.AnyAsync(c => c.Id == dto.CompraId);
        if (!compraExists)
            return (false, "Compra não encontrada.");

        var despesaExists = await _context.Despesas.AnyAsync(d => d.Id == dto.DespesaId);
        if (!despesaExists)
            return (false, "Despesa não encontrada.");

        return (true, string.Empty);
    }
}