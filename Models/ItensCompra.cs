using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models
{
    [Table("itens_compra")]
    public class ItensCompra
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("valor")]
        public double Valor { get; set; }

        [Required]
        [Column("produto_id")]
        public int ProdutoId { get; set; }

        [Required]
        [Column("compra_id")]
        public int CompraId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto Produto { get; set; }

        [ForeignKey("CompraId")]
        public virtual Compra Compra { get; set; }
    }
}
