using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
[Table("pagamentos")]
public class Pagamento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("data")]
    public DateTime Data { get; set; }

    [Column("valor")]
    public double Valor { get; set; }

    [Column("forma_pag")]
    public string FormaPag { get; set; }

    [ForeignKey("caixa_id")]
    [Column("caixa_id")]
    public Caixa CaixaId { get; set; }

    [ForeignKey("compra_id")]
    [Column("compra_id")]
    public Compra CompraId { get; set; }

    [ForeignKey("despesa_id")]
    [Column("despesa_id")]
    public Despesa DespesaId { get; set; }

    [ForeignKey("funcionario_id")]
    [Column("funcionario_id")]
    public Funcionario FuncionarioId { get; set; }


    public Caixa Caixa { get; set; }
    public Compra Compra { get; set; }
    public Despesa Despesa { get; set; }
    public Funcionario Funcionario { get; set; }
}