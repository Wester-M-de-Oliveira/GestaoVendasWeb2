using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    [Column("preco_compra")]
    public decimal PrecoCompra { get; set; }

    [Column("valor_prod")]
    public decimal Valor { get; set; }

    [Column("quantidade_estoque")]
    public int QuantidadeEstoque { get; set; }

    [Column("data_validade")]
    public DateTime? DataValidade { get; set; }

    [JsonIgnore]
    public ICollection<ItensCompra> ItensCompras { get; set; }

    [JsonIgnore]
    public ICollection<ItensVenda> ItensVendas { get; set; }

}

