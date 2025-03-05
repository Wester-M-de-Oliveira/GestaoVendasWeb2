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
                CidadeId = item.Endereco.CidadeId
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
        public async Task<IActionResult> Put(int id, [FromBody] UpdateFuncionarioDTO item)
        {
            try
            {
                var funcionario = await _context.Funcionarios.Include(f => f.Endereco)
                        .ThenInclude(e => e.Cidade)
                        .ThenInclude(c => c.Estado)
                        .FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario == null) return NotFound("Funcionário não encontrado.");

                if (!string.IsNullOrEmpty(item.Nome))
                    funcionario.Nome = item.Nome;

                if (!string.IsNullOrEmpty(item.CPF))
                    funcionario.CPF = item.CPF;

                if (!string.IsNullOrEmpty(item.RG))
                    funcionario.RG = item.RG;

                if (item.DataNascimento.HasValue)
                    funcionario.DataNascimento = item.DataNascimento.Value;

                if (item.Salario.HasValue)
                    funcionario.Salario = item.Salario.Value;

                if (!string.IsNullOrEmpty(item.Funcao))
                    funcionario.Funcao = item.Funcao;

                if (!string.IsNullOrEmpty(item.Telefone))
                    funcionario.Telefone = item.Telefone;

                if (!string.IsNullOrEmpty(item.Sexo))
                    funcionario.Sexo = item.Sexo;

                if (item.Endereco != null)
                {
                    if (!string.IsNullOrEmpty(item.Endereco.Rua))
                        funcionario.Endereco.Rua = item.Endereco.Rua;

                    if (item.Endereco.Numero.HasValue && item.Endereco.Numero > 0)
                        funcionario.Endereco.Numero = item.Endereco.Numero.Value;

                    if (!string.IsNullOrEmpty(item.Endereco.Bairro))
                        funcionario.Endereco.Bairro = item.Endereco.Bairro;

                    if (item.Endereco.CidadeId.HasValue && item.Endereco.CidadeId > 0)
                        funcionario.Endereco.CidadeId = item.Endereco.CidadeId.Value;
                }

                _context.Funcionarios.Update(funcionario);
                await _context.SaveChangesAsync();

                return Ok(funcionario);
            }
            catch (Exception e)
            {
                return Problem($"Erro ao atualizar funcionário: {e.Message}");
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
