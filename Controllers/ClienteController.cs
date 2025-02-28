﻿using GestaoVendasWeb2.DataContexts;
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
        public async Task<IActionResult> Put(int id, [FromBody] UpdateClienteDTO item)
        {
            try
            {
                var cliente = await _context.Clientes.Include(c => c.Endereco)
                        .ThenInclude(e => e.Cidade)
                        .ThenInclude(c => c.Estado)
                        .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null) return NotFound("Cliente não encontrado.");

                if (!string.IsNullOrEmpty(item.Nome))
                    cliente.Nome = item.Nome;

                if (!string.IsNullOrEmpty(item.EstadoCivil))
                    cliente.EstadoCivil = item.EstadoCivil;

                if (!string.IsNullOrEmpty(item.Cpf))
                    cliente.CPF = item.Cpf;

                if (!string.IsNullOrEmpty(item.Rg))
                    cliente.RG = item.Rg;

                if (item.DataNasc.HasValue)
                    cliente.DataNascimento = item.DataNasc;

                if (item.RendaFamiliar.HasValue)
                    cliente.RendaFamiliar = (float?)item.RendaFamiliar;

                if (!string.IsNullOrEmpty(item.Telefone))
                    cliente.Telefone = item.Telefone;

                if (!string.IsNullOrEmpty(item.Sexo))
                    cliente.Sexo = item.Sexo;

                if (!string.IsNullOrEmpty(item.Celular))
                    cliente.Celular = item.Celular;

                if (item.Endereco != null)
                {
                    if (!string.IsNullOrEmpty(item.Endereco.Rua))
                        cliente.Endereco.Rua = item.Endereco.Rua;

                    if (item.Endereco.Numero.HasValue && item.Endereco.Numero > 0)
                        cliente.Endereco.Numero = item.Endereco.Numero.Value;

                    if (!string.IsNullOrEmpty(item.Endereco.Bairro))
                        cliente.Endereco.Bairro = item.Endereco.Bairro;

                    if (item.Endereco.CidadeId.HasValue && item.Endereco.CidadeId > 0)
                        cliente.Endereco.CidadeId = item.Endereco.CidadeId.Value;
                }

                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();

                return Ok(cliente);
            }
            catch (Exception e)
            {
                return Problem($"Erro ao atualizar cliente: {e.Message}");
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
