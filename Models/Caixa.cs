using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models 
{
    [Table("caixa")]
    public class Caixa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("data_abertura")]
        public DateTime DataAbertura { get; set; }

        [Column("data_fechamento")]
        public DateTime DataFechamento { get; set; }

        [Column("saldo_inicial")]
        public double SaldoIncial { get; set; }

        [Column("troco")]
        public double Troco { get; set; }

        [ForeignKey("valor_creditos")]
        public double ValorCreditos { get; set; }

        [ForeignKey("valor_debitos")]
        public double ValorDebitos { get; set; }

        [ForeignKey("saldo_final")]
        public double SaldoFinal { get; set; }

        [ForeignKey("status")]
        public bool Status { get; set; }
    }
}

