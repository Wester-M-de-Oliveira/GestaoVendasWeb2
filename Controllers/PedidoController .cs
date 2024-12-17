using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("pedidos")]
    [ApiController]
    public class PedidoController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Pedido> pedidos = new PedidoDAO().List();
                return Ok(pedidos);
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
                var pedido = new PedidoDAO().GetById(id);

                if (pedido == null) return NotFound();

                return Ok(pedido);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] PedidoDTO item)
        {
            var pedido = new Pedido
            {
                Data = item.Data,
                Total = item.Total,
                IdCliente = item.IdCliente
            };

            try
            {
                var dao = new PedidoDAO();
                pedido.IdPedido = dao.Insert(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("", pedido);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PedidoDTO item)
        {
            try
            {
                var pedido = new PedidoDAO().GetById(id);
                if (pedido == null) return NotFound();

                pedido.Data = item.Data;
                pedido.Total = item.Total;
                pedido.IdCliente = item.IdCliente;

                new PedidoDAO().Update(pedido);

                return Ok(pedido);
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
                var pedido = new PedidoDAO().GetById(id);
                if (pedido == null) return NotFound();

                new PedidoDAO().Delete(pedido.IdPedido);

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
