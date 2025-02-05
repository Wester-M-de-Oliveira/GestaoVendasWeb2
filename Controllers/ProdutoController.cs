using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("produtos")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var produtos = await _context.Produtos.ToListAsync();
                return Ok(produtos);
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
                Preco = item.Preco,
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProdutoDTO item)
        {
            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null) return NotFound();

                produto.Nome = item.Nome;
                produto.Descricao = item.Descricao;
                produto.Preco = item.Preco;
                produto.QuantidadeEstoque = item.QuantidadeEstoque;
                produto.DataValidade = item.DataValidade;

                await _context.SaveChangesAsync();
                return Ok(produto);
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