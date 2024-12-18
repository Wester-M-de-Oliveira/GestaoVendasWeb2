using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;

[Table("compra")]
public class Compra
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

    [ForeignKey("funcionario_id")]
    [Column("funcionario_id")]
    public Funcionario FuncionarioId { get; set; }

    [ForeignKey("fornecedor_id")]
    [Column("fornecedor_id")]
    public Fornecedor FornecedorId { get; set; }

    public Funcionario Funcionario { get; set; }
    public Fornecedor Fornecedor { get; set; }
    public ICollection<ItensCompra> ItensCompras { get; set; }
    public ICollection<Pagamento> Pagamentos { get; set; }
}