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
        [Required]
        public DateTime Data { get; set; } = DateTime.Now;

        [Column("valor")]
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Valor { get; set; }

        [Column("forma_pag")]
        [Required]
        [StringLength(50)]
        public string FormaPag { get; set; }

        [Column("status")]
        [Required]
        public StatusCompra Status { get; set; } = StatusCompra.EmAberto;

        [Column("observacao")]
        [StringLength(500)]
        public string Observacao { get; set; }

        [ForeignKey("funcionario_id")]
        [Required]
        public int FuncionarioId { get; set; }

        [ForeignKey("fornecedor_id")]
        [Required]
        public int FornecedorId { get; set; }

        public virtual Funcionario Funcionario { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual ICollection<ItensCompra> ItensCompras { get; set; } = [];
        public virtual ICollection<Pagamento> Pagamentos { get; set; } = [];

        public void AdicionarItem(ItensCompra item)
        {
            ItensCompras.Add(item);
            RecalcularValorTotal();
        }

        public void RemoverItem(ItensCompra item)
        {
            ItensCompras.Remove(item);
            RecalcularValorTotal();
        }

        public void RecalcularValorTotal()
        {
            Valor = (decimal)ItensCompras.Sum(item => item.Valor * item.Quantidade);
        }

        public void FinalizarCompra()
        {
            if (!ItensCompras.Any())
                throw new InvalidOperationException("Não é possível finalizar uma compra sem itens.");

            if (Valor <= 0)
                throw new InvalidOperationException("Valor da compra inválido.");

            Status = StatusCompra.Finalizada;
        }

        public void CancelarCompra()
        {
            Status = StatusCompra.Cancelada;
        }
    }

    public enum StatusCompra
    {
        EmAberto,
        Finalizada,
        Cancelada
    }
}