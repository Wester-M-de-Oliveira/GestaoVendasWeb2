using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models 
{
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
        public Funcionario Funcionario { get; set; }

        [ForeignKey("fornecedor_id")]
        public Fornecedor Fornecedor { get; set; }
    }
}
