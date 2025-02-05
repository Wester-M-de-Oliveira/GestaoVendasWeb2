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

        [Required]
        [Column("data_abertura")]
        public DateTime DataAbertura { get; set; } = DateTime.Now;

        [Column("data_fechamento")]
        public DateTime? DataFechamento { get; set; }

        [Required]
        [Column("saldo_inicial")]
        [Range(0, double.MaxValue)]
        public decimal SaldoInicial { get; set; }

        [Column("troco")]
        [Range(0, double.MaxValue)]
        public decimal Troco { get; set; }

        [Column("valor_creditos")]
        [Range(0, double.MaxValue)]
        public decimal ValorCreditos { get; set; }

        [Column("valor_debitos")]
        [Range(0, double.MaxValue)]
        public decimal ValorDebitos { get; set; }

        [Column("saldo_final")]
        public decimal SaldoFinal { get; set; }

        [Required]
        [Column("status")]
        public bool Status { get; set; } = true;

        public virtual ICollection<Recebimento> Recebimentos { get; set; } = new List<Recebimento>();
        public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

        public void CalcularSaldoFinal()
        {
            SaldoFinal = SaldoInicial + ValorCreditos - ValorDebitos;
        }

        public void Fechar()
        {
            if (Status)
            {
                DataFechamento = DateTime.Now;
                Status = false;
                CalcularSaldoFinal();
            }
        }
    }
}