using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
[Table("venda")]
public class Venda
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("data_venda")]
    public DateTime Data { get; set; }

    [Column("valor")]
    public double Valor { get; set; }

    [Column("desconto")]
    public double Desconto { get; set; }

    [Column("forma_pag")]
    public string FormaPag { get; set; }

    [ForeignKey("quant_parcelas")]
    public int QuantParcelas { get; set; }

    [ForeignKey("funcionario_id")]
    public Funcionario Funcionario { get; set; }

    [ForeignKey("cliente_id")]
    public Cliente Cliente { get; set; }
}
