using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("itens-pedido")]
    [ApiController]
    public class ItemPedidoController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<ItemPedido> itensPedido = new ItemPedidoDAO().List();
                return Ok(itensPedido);
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
                var itemPedido = new ItemPedidoDAO().GetById(id);

                if (itemPedido == null) return NotFound();

                return Ok(itemPedido);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ItemPedidoDTO item)
        {
            var itemPedido = new ItemPedido
            {
                IdPedido = item.IdPedido,
                IdProduto = item.IdProduto,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            };

            try
            {
                var dao = new ItemPedidoDAO();
                itemPedido.IdItemPedido = dao.Insert(itemPedido);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("", itemPedido);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ItemPedidoDTO item)
        {
            try
            {
                var itemPedido = new ItemPedidoDAO().GetById(id);
                if (itemPedido == null) return NotFound();

                itemPedido.IdPedido = item.IdPedido;
                itemPedido.IdProduto = item.IdProduto;
                itemPedido.Quantidade = item.Quantidade;
                itemPedido.PrecoUnitario = item.PrecoUnitario;

                new ItemPedidoDAO().Update(itemPedido);

                return Ok(itemPedido);
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
                var itemPedido = new ItemPedidoDAO().GetById(id);
                if (itemPedido == null) return NotFound();

                new ItemPedidoDAO().Delete(itemPedido.IdItemPedido);

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
