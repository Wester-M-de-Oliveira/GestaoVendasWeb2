using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers
{
    [Route("clientes")]
    [ApiController]
    public class ClienteController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Include(c => c.Endereco)
                        .ThenInclude(e => e.Cidade)
                            .ThenInclude(c => c.Estado)
                    .ToListAsync();
                return Ok(clientes);
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
                var cliente = await _context.Clientes.Include(c => c.Endereco).FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null) return NotFound();

                return Ok(cliente);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteDTO item)
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

            var cliente = new Cliente
            {
                Nome = item.Nome,
                EstadoCivil = item.EstadoCivil,
                CPF = item.Cpf,
                RG = item.Rg,
                DataNascimento = item.DataNasc,
                RendaFamiliar = (float?)item.RendaFamiliar,
                Telefone = item.Telefone,
                Sexo = item.Sexo,
                Celular = item.Celular,
                Endereco = endereco
            };

            try
            {
                _context.Enderecos.Add(endereco);
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao salvar cliente", erro = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ClienteDTO item)
        {
            try
            {
                var cliente = await _context.Clientes.Include(c => c.Endereco).FirstOrDefaultAsync(c => c.Id == id);
                if (cliente == null) return NotFound();

                cliente.Nome = item.Nome;
                cliente.EstadoCivil = item.EstadoCivil;
                cliente.CPF = item.Cpf;
                cliente.RG = item.Rg;
                cliente.DataNascimento = item.DataNasc;
                cliente.RendaFamiliar = (float?)item.RendaFamiliar;
                cliente.Telefone = item.Telefone;
                cliente.Sexo = item.Sexo;
                cliente.Celular = item.Celular;

                cliente.Endereco.Rua = item.Endereco.Rua;
                cliente.Endereco.Numero = item.Endereco.Numero;
                cliente.Endereco.Bairro = item.Endereco.Bairro;
                cliente.Endereco.CidadeId = item.Endereco.CidadeId;

                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();

                return Ok(cliente);
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
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null) return NotFound();

                _context.Clientes.Remove(cliente);
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
