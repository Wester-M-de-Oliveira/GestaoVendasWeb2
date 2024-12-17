using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class ProdutoFornecedorDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public int IdProduto { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public Produto Produto { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public Fornecedor Fornecedor { get; set; }
    }
}
