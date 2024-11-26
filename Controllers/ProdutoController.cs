using System;

[Route("produtos")]
[ApiController]
public class ProdutoController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            List<Produto> produtos = new ProdutoDAO().List();
            return Ok(produtos);
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
            var produto = new ProdutoDAO().GetById(id);

            if (produto == null) return NotFound();

            return Ok(produto);
        }
        catch (Exception)
        {
            return Problem("Erro ao processar a solicitação.");
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] ProdutoDTO item)
    {
        var produto = new Produto
        {
            Nome = item.Nome,
            Preco = item.Preco,
            QuantidadeEstoque = item.QuantidadeEstoque,
            DataValidade = item.DataValidade
        };

        try
        {
            var dao = new ProdutoDAO();
            produto.IdProduto = dao.Insert(produto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Created("", produto);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ProdutoDTO item)
    {
        try
        {
            var produto = new ProdutoDAO().GetById(id);
            if (produto == null) return NotFound();

            produto.Nome = item.Nome;
            produto.Preco = item.Preco;
            produto.QuantidadeEstoque = item.QuantidadeEstoque;
            produto.DataValidade = item.DataValidade;

            new ProdutoDAO().Update(produto);

            return Ok(produto);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var produto = new ProdutoDAO().GetById(id);
            if (produto == null) return NotFound();

            new ProdutoDAO().Delete(produto.IdProduto);

            return Ok();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}