using System;

public class ProdutoFornecedorDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    public int IdProduto { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public int IdFornecedor { get; set; }
}
