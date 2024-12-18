using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
[Table("itens_compra")]
public class ItensCompra
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("quantidade")]
    public int quantidade { get; set; }

    [Column("valor")]
    public double Valor { get; set; }

    [Column("produto_id")]
    public Produto Produto { get; set; }

    [ForeignKey("compra_id")]
    public Compra Compra { get; set; }
}
