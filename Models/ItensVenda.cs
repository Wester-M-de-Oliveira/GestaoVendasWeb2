using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;

[Table("itens_venda")]
public class ItensVenda
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("quantidade")]
    public int quantidade { get; set; }

    [Column("valor")]
    public double Valor { get; set; }

    [ForeignKey("produto_id")]
    [Column("produto_id")]
    public Produto ProdutoId { get; set; }

    [ForeignKey("venda_id")]
    [Column("venda_id")]
    public Venda VendaId { get; set; }

    public Produto Produto { get; set; }
    public Venda Venda { get; set; }
}