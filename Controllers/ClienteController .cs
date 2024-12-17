using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers 
{
    [Route("clientes")]
    [ApiController]
    public class ClienteController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Cliente> clientes = new ClienteDAO().List();
                return Ok(clientes);
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
                var cliente = new ClienteDAO().GetById(id);

                if (cliente == null) return NotFound();

                return Ok(cliente);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClienteDTO item)
        {
            var cliente = new Cliente
            {
                Nome = item.Nome,
                Telefone = item.Telefone,
                Endereco = item.Endereco,
                Email = item.Email
            };

            try
            {
                var dao = new ClienteDAO();
                cliente.IdCliente = dao.Insert(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("", cliente);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ClienteDTO item)
        {
            try
            {
                var cliente = new ClienteDAO().GetById(id);
                if (cliente == null) return NotFound();

                cliente.Nome = item.Nome;
                cliente.Telefone = item.Telefone;
                cliente.Endereco = item.Endereco;
                cliente.Email = item.Email;

                new ClienteDAO().Update(cliente);

                return Ok(cliente);
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
                var cliente = new ClienteDAO().GetById(id);
                if (cliente == null) return NotFound();

                new ClienteDAO().Delete(cliente.IdCliente);

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
