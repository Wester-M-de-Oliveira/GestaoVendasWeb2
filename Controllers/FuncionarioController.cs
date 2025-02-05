using GestaoVendasWeb2.DataContexts;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.Controllers
{
    [Route("funcionarios")]
    [ApiController]
    public class FuncionarioController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var funcionarios = await _context.Funcionarios.Include(f => f.Endereco).ToListAsync();
                return Ok(funcionarios);
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
                var funcionario = await _context.Funcionarios.Include(f => f.Endereco).FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario == null) return NotFound();

                return Ok(funcionario);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FuncionarioDTO item)
        {
            var endereco = new Endereco {
                Rua = item.Endereco.Rua,
                Numero = item.Endereco.Numero,
                Bairro = item.Endereco.Bairro,
                Cidade = item.Endereco.Cidade,
            };

            var funcionario = new Funcionario {
                Nome = item.Nome,
                CPF = item.CPF,
                RG = item.RG,
                DataNascimento = item.DataNascimento,
                Salario = item.Salario,
                Funcao = item.Funcao,
                Telefone = item.Telefone,
                Sexo = item.Sexo,
                Endereco = endereco
            };

            try
            {
                _context.Enderecos.Add(endereco);
                _context.Funcionarios.Add(funcionario);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetById), new { id = funcionario.Id }, funcionario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FuncionarioDTO item)
        {
            try
            {
                var funcionario = await _context.Funcionarios.Include(f => f.Endereco).FirstOrDefaultAsync(f => f.Id == id);
                if (funcionario == null) return NotFound();

                funcionario.Nome = item.Nome;
                funcionario.CPF = item.CPF;
                funcionario.RG = item.RG;
                funcionario.DataNascimento = item.DataNascimento;
                funcionario.Salario = item.Salario;
                funcionario.Funcao = item.Funcao;
                funcionario.Telefone = item.Telefone;
                funcionario.Sexo = item.Sexo;

                funcionario.Endereco.Rua = item.Endereco.Rua;
                funcionario.Endereco.Numero = item.Endereco.Numero;
                funcionario.Endereco.Bairro = item.Endereco.Bairro;
                funcionario.Endereco.Cidade = item.Endereco.Cidade;

                _context.Funcionarios.Update(funcionario);
                await _context.SaveChangesAsync();

                return Ok(funcionario);
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
                var funcionario = await _context.Funcionarios.FindAsync(id);
                if (funcionario == null) return NotFound();

                _context.Funcionarios.Remove(funcionario);
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
