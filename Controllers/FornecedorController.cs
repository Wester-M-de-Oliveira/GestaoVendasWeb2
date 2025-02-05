using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("fornecedores")]
    [ApiController]
    public class FornecedorController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var fornecedores = await _context.Fornecedores.Include(f => f.Endereco).ToListAsync();
                return Ok(fornecedores);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var fornecedor = await _context.Fornecedores
                    .Include(f => f.Endereco)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fornecedor == null) return NotFound();

                return Ok(fornecedor);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FornecedorDTO dto)
        {
            try
            {
                var fornecedor = new Fornecedor
                {
                    RazaoSocial = dto.RazaoSocial,
                    NomeFantasia = dto.NomeFantasia,
                    Endereco = dto.Endereco
                };

                _context.Fornecedores.Add(fornecedor);
                await _context.SaveChangesAsync();

                return Created("", fornecedor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FornecedorDTO dto)
        {
            try
            {
                var fornecedor = await _context.Fornecedores
                    .Include(f => f.Endereco)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fornecedor == null) return NotFound();

                fornecedor.RazaoSocial = dto.RazaoSocial;
                fornecedor.NomeFantasia = dto.NomeFantasia;
                fornecedor.Endereco = dto.Endereco;

                await _context.SaveChangesAsync();
                return Ok(fornecedor);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var fornecedor = await _context.Fornecedores.FindAsync(id);
                if (fornecedor == null) return NotFound();

                _context.Fornecedores.Remove(fornecedor);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
