using System.Collections.Generic;

public class ProdutoFornecedor
{
    public int IdProduto { get; set; }
    public Produto Produto { get; set; }
    public int IdFornecedor { get; set; }
    public Fornecedor Fornecedor { get; set; }
}
