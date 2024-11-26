using System;

public class Produto
{
    public int IdProduto { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public DateTime? DataValidade { get; set; }
    public ICollection<ItemPedido> ItensPedidos { get; set; }
    public ICollection<ProdutoFornecedor> Fornecedores { get; set; }
}
