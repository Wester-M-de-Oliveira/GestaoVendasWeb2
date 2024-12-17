using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models 
{
    [Table("despesa")]
    public class Despesa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("valor")]
        public double Valor { get; set; }

        [Column("data_vencimento")]
        public DateTime DataVencimento { get; set; }

        [Column("numero_doc")]
        public int NumeroDoc { get; set; }

        [ForeignKey("fornecedor_id")]
        public Fornecedor Fornecedor { get; set; }
    }
}

