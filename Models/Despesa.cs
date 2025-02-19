using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;
[Table("despesa")]
public class Despesa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("descricao")]
    [StringLength(200)]
    public string Descricao { get; set; }

    [Required]
    [Column("valor")]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    [Column("data_vencimento")]
    public DateTime DataVencimento { get; set; }

    [Required]
    [Column("numero_doc")]
    public int NumeroDoc { get; set; }

    [Required]
    [Column("fornecedor_id")]
    public int FornecedorId { get; set; }

    public virtual Fornecedor Fornecedor { get; set; }
    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

    public decimal CalcularValorPendente()
    {
        return Valor - Pagamentos.Sum(static p => (decimal)p.Valor);
    }
}