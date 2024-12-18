using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models
{
    [Table("pagamentos")]
    public class Pagamentos
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

        [Column("caixa_id")]
        public Caixa Caixa { get; set; }

        [Column("compra_id")]
        public Compra Compra { get; set; }

        [Column("despesa_id")]
        public Despesas Despesas { get; set; }

        [ForeignKey("funcionario_id")]
        public Funcionario Funcionario { get; set; }
    }
}
