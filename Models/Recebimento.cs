using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models
{
    [Table("recebimento")]
    public class Recebimento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("data")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required]
        [Column("valor")]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        [Required]
        [Column("caixa_id")]
        public int CaixaId { get; set; }

        [Required]
        [Column("venda_id")]
        public int VendaId { get; set; }

        [Required]
        [Column("funcionario_id")]
        public int FuncionarioId { get; set; }

        public virtual Caixa Caixa { get; set; }
        public virtual Venda Venda { get; set; }
        public virtual Funcionario Funcionario { get; set; }

        public void ValidarRecebimento()
        {
            if (Valor <= 0)
                throw new InvalidOperationException("O valor do recebimento deve ser maior que zero.");
            
            if (Data > DateTime.Now)
                throw new InvalidOperationException("A data do recebimento não pode ser futura.");
        }
    }
}