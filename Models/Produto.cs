using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models;

[Table("produto")]
public class Produto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("preco")]
        public decimal Preco { get; set; }

        [Column("quantidade_estoque")]
        public int QuantidadeEstoque { get; set; }

        [Column("data_validade")]
        public DateTime? DataValidade { get; set; }
    }
}

