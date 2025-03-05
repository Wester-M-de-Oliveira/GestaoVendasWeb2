using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("produtos")]
    [ApiController]
    public class ProdutoController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var produtos = await _context.Produtos.ToListAsync();
                return Ok(produtos);
            }
            catch (Exception e)
            {
                return Problem("Erro ao processar a solicitação.", e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null) return NotFound();
                return Ok(produto);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProdutoDTO item)
        {
            var produto = new Produto
            {
                Nome = item.Nome,
                Descricao = item.Descricao,
                PrecoCompra = item.PrecoCompra,
                Valor = item.Valor,
                QuantidadeEstoque = item.QuantidadeEstoque,
                DataValidade = item.DataValidade
            };

            try
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                return Created($"produtos/{produto.Id}", produto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] ProdutoUpdateDTO item)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null) return NotFound("Produto não encontrado.");

                if (!string.IsNullOrEmpty(item.Nome))
                    produto.Nome = item.Nome;

                if (!string.IsNullOrEmpty(item.Descricao))
                    produto.Descricao = item.Descricao;

                if (item.PrecoCompra > 0)
                    produto.PrecoCompra = (decimal)item.PrecoCompra;

                if (item.Valor > 0)
                    produto.Valor = (decimal)item.Valor;

                if (item.QuantidadeEstoque >= 0)
                    produto.QuantidadeEstoque = (int)item.QuantidadeEstoque;

                if (item.DataValidade.HasValue)
                    produto.DataValidade = item.DataValidade;

                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();

                return Ok(produto);
            }
            catch (Exception e)
            {
                return Problem($"Erro ao atualizar o produto: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null) return NotFound();

                _context.Produtos.Remove(produto);
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