using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using AutoMapper;

namespace GestaoVendasWeb2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DespesaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DespesaController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DespesaDTO>>> GetDespesas()
    {
        var despesas = await _context.Despesas
            .Include(d => d.Fornecedor)
            .Include(d => d.Pagamentos)
            .ToListAsync();

        return Ok(_mapper.Map<List<DespesaDTO>>(despesas));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DespesaDTO>> GetDespesa(int id)
    {
        var despesa = await _context.Despesas
            .Include(d => d.Fornecedor)
            .Include(d => d.Pagamentos)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (despesa == null)
            return NotFound($"Despesa com ID {id} não encontrada.");

        return _mapper.Map<DespesaDTO>(despesa);
    }

    [HttpPost]
    public async Task<ActionResult<DespesaDTO>> PostDespesa(DespesaDTO despesaDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == despesaDto.FornecedorId);
            if (!fornecedorExists)
                return BadRequest("Fornecedor não encontrado.");

            var despesa = _mapper.Map<Despesa>(despesaDto);
            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetDespesa), new { id = despesa.Id }, _mapper.Map<DespesaDTO>(despesa));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao criar despesa: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDespesa(int id, DespesaDTO despesaDto)
    {
        if (id != despesaDto.Id)
            return BadRequest("O ID da URL não corresponde ao ID do objeto.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var despesa = await _context.Despesas.FindAsync(id);
            if (despesa == null)
                return NotFound($"Despesa com ID {id} não encontrada.");

            var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == despesaDto.FornecedorId);
            if (!fornecedorExists)
                return BadRequest("Fornecedor não encontrado.");

            _mapper.Map(despesaDto, despesa);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(_mapper.Map<DespesaDTO>(despesa));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao atualizar despesa: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDespesa(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var despesa = await _context.Despesas
                .Include(d => d.Pagamentos)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (despesa == null)
                return NotFound($"Despesa com ID {id} não encontrada.");

            if (despesa.Pagamentos.Any())
                return BadRequest("Não é possível excluir uma despesa que possui pagamentos.");

            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erro ao excluir despesa: {ex.Message}");
        }
    }
}