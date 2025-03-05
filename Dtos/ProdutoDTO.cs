using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class ProdutoDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal PrecoCompra { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade em estoque deve ser não-negativa")]
        public int QuantidadeEstoque { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataValidade { get; set; }
    }

    public class ProdutoUpdateDTO
    {
        [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal? PrecoCompra { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal? Valor { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade em estoque deve ser não-negativa")]
        public int? QuantidadeEstoque { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataValidade { get; set; }
    }
}
