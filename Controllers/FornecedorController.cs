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
        public async Task<IActionResult> Post([FromBody] FornecedorDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var endereco = new Endereco
            {
                Rua = item.Endereco.Rua,
                Numero = item.Endereco.Numero,
                Bairro = item.Endereco.Bairro,
                CidadeId = item.Endereco.CidadeId
            };
            var fornecedor = new Fornecedor
            {
                RazaoSocial = item.RazaoSocial,
                NomeFantasia = item.NomeFantasia,
                Endereco = endereco
            };
            try
            {
                _context.Enderecos.Add(endereco);
                _context.Fornecedores.Add(fornecedor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao salvar Fornecedor", erro = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FornecedorUpdateDTO item)
        {
            try
            {
            var fornecedor = await _context.Fornecedores
                .Include(f => f.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornecedor == null) return NotFound("Fornecedor não encontrado.");

            if (!string.IsNullOrEmpty(item.RazaoSocial))
                fornecedor.RazaoSocial = item.RazaoSocial;

            if (!string.IsNullOrEmpty(item.NomeFantasia))
                fornecedor.NomeFantasia = item.NomeFantasia;

            if (item.Endereco != null)
            {
                if (!string.IsNullOrEmpty(item.Endereco.Rua))
                fornecedor.Endereco.Rua = item.Endereco.Rua;

                if (item.Endereco.Numero.HasValue && item.Endereco.Numero > 0)
                fornecedor.Endereco.Numero = item.Endereco.Numero.Value;

                if (!string.IsNullOrEmpty(item.Endereco.Bairro))
                fornecedor.Endereco.Bairro = item.Endereco.Bairro;

                if (item.Endereco.CidadeId.HasValue && item.Endereco.CidadeId > 0)
                fornecedor.Endereco.CidadeId = item.Endereco.CidadeId.Value;
            }

            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();

            return Ok(fornecedor);
            }
            catch (Exception e)
            {
            return Problem($"Erro ao atualizar fornecedor: {e.Message}");
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
