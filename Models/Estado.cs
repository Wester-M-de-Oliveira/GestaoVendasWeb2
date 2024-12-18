using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
public class Estado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("nome")]
    public string Nome { get; set; }
    [Column("sigla")]
    public string Sigla { get; set; }

    public ICollection<Cidade> Cidades { get; set; }
}
