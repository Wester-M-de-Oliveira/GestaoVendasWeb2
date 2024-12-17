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

        [Column("caixa_id")]
        public Caixa Caixa { get; set; }

        [ForeignKey("venda_id")]
        public Venda Venda { get; set; }

        [ForeignKey("funcionario_id")]
        public Funcionario Funcionario { get; set; }
    }
}
