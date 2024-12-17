using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("produto-fornecedores")]
    [ApiController]
    public class ProdutoFornecedorController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<ProdutoFornecedor> produtoFornecedores = new ProdutoFornecedorDAO().List();
                return Ok(produtoFornecedores);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var produtoFornecedor = new ProdutoFornecedorDAO().GetById(id);

                if (produtoFornecedor == null) return NotFound();

                return Ok(produtoFornecedor);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoFornecedorDTO item)
        {
            var produtoFornecedor = new ProdutoFornecedor
            {
                IdProduto = item.IdProduto,
                IdFornecedor = item.IdFornecedor
            };

            try
            {
                var dao = new ProdutoFornecedorDAO();
                dao.Insert(produtoFornecedor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("", produtoFornecedor);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var produtoFornecedor = new ProdutoFornecedorDAO().GetById(id);
                if (produtoFornecedor == null) return NotFound();

                new ProdutoFornecedorDAO().Delete(id);

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
