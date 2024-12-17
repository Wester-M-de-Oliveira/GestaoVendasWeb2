using GestaoVendasWeb2.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoVendasWeb2.DataContexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
    }

}
