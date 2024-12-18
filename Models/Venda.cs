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
    [Column("funcionario_id")]
    public Funcionario FuncionarioId { get; set; }

    [ForeignKey("cliente_id")]
    [Column("cliente_id")]
    public Cliente ClienteId { get; set; }

    public Funcionario Funcionario { get; set; }
    public Cliente Cliente { get; set; }
    public ICollection<ItensVenda> ItensVendas { get; set; }
    public ICollection<Recebimento> Recebimentos { get; set; }
}