using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;

[Table("recebimento")]
public class Recebimento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("data")]
    public DateTime Data { get; set; }

    [Column("valor")]
    public double Valor { get; set; }

    [ForeignKey("caixa_id")]
    [Column("caixa_id")]
    public Caixa CaixaId { get; set; }

    [ForeignKey("venda_id")]
    [Column("venda_id")]
    public Venda VendaId { get; set; }

    [ForeignKey("funcionario_id")]
    [Column("funcionario_id")]
    public Funcionario FuncionarioId { get; set; }

    public Caixa Caixa { get; set; }
    public Venda Venda { get; set; }
    public Funcionario Funcionario { get; set; }
}