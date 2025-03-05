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
        public DateTime Data { get; set; } = DateTime.Now;

        [Column("valor")]
        [Range(0, double.MaxValue)]
        public decimal Valor { get; set; }

        [Column("forma_pag")]
        [StringLength(50)]
        public string FormaPag { get; set; }

        [Column("status")]
        public StatusCompra Status { get; set; }

        [Column("observacao")]
        [StringLength(200)]
        public string Observacao { get; set; }

        [ForeignKey("funcionario_id")]
        [Column("funcionario_id")]
        public int FuncionarioId { get; set; }
        public virtual Funcionario Funcionario { get; set; }

        [ForeignKey("fornecedor_id")]
        [Column("fornecedor_id")]
        public int FornecedorId { get; set; }
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