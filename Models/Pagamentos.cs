using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
[Table("pagamento")]
public class Pagamento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("data")]
    public DateTime Data { get; set; } = DateTime.Now;

    [Required]
    [Column("valor")]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    [Column("forma_pag")]
    [StringLength(50)]
    public string FormaPag { get; set; }

    [Required]
    [Column("caixa_id")]
    public int CaixaId { get; set; }

    [Required]
    [Column("compra_id")]
    public int CompraId { get; set; }

    [Required]
    [Column("despesa_id")]
    public int DespesaId { get; set; }

    [Required]
    [Column("funcionario_id")]
    public int FuncionarioId { get; set; }

    public virtual Caixa Caixa { get; set; }
    public virtual Compra Compra { get; set; }
    public virtual Despesa Despesa { get; set; }
    public virtual Funcionario Funcionario { get; set; }
}