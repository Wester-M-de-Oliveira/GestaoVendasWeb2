using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models
{
    [Table("venda")]
    public class Venda
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("data_venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Required]
        [Column("valor")]
        [Range(0, double.MaxValue)]
        public decimal Valor { get; set; }

        [Column("desconto")]
        [Range(0, double.MaxValue)]
        public decimal? Desconto { get; set; }

        [Required]
        [Column("forma_pag")]
        [StringLength(50)]
        public string FormaPag { get; set; }

        [Column("quant_parcelas")]
        [Range(0, int.MaxValue)]
        public int? QuantParcelas { get; set; }

        [Required]
        [Column("funcionario_id")]
        public int FuncionarioId { get; set; }

        [Required]
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        public virtual Funcionario Funcionario { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<ItensVenda> ItensVendas { get; set; } = new List<ItensVenda>();
        public virtual ICollection<Recebimento> Recebimentos { get; set; } = new List<Recebimento>();

        public void CalcularValorTotal()
        {
            Valor = ItensVendas.Sum(item => (decimal)item.Valor * item.Quantidade);
            if (Desconto.HasValue)
            {
                Valor -= Desconto.Value;
            }
        }

        public void AdicionarItem(ItensVenda item)
        {
            ItensVendas.Add(item);
            CalcularValorTotal();
        }

        public void RemoverItem(ItensVenda item)
        {
            ItensVendas.Remove(item);
            CalcularValorTotal();
        }

        public decimal CalcularValorRestante()
        {
            decimal valorPago = (decimal)Recebimentos.Sum(r => r.Valor);
            return Valor - valorPago;
        }
    }
}