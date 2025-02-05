using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.DataContexts;

namespace GestaoVendasWeb2.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CidadeController : ControllerBase
{
    private readonly AppDbContext _context;

    public CidadeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cidade>>> GetCidades()
    {
        return await _context.Cidades.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cidade>> GetCidade(int id)
    {
        var cidade = await _context.Cidades.FindAsync(id);

        if (cidade == null)
        {
            return NotFound();
        }

        return cidade;
    }
}
