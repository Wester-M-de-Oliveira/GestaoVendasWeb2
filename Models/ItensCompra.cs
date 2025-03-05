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

        [Required]
        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Required]
        [Column("valor")]
        public decimal Valor { get; set; }

        [Required]
        [Column("produto_id")]
        public int ProdutoId { get; set; }

        [Required]
        [Column("compra_id")]
        public int CompraId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto Produto { get; set; } = null!;

        [ForeignKey("CompraId")]
        public virtual Compra Compra { get; set; } = null!;
    }
}
