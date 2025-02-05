using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class VendaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A data da venda é obrigatória")]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.Date)]
        public DateTime DataVenda { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Display(Name = "Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public double Valor { get; set; }

        [Display(Name = "Desconto")]
        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public double? Desconto { get; set; }

        [Display(Name = "Forma de Pagamento")]
        [StringLength(50)]
        public string FormaPag { get; set; }

        [Display(Name = "Quantidade de Parcelas")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de parcelas não pode ser negativa")]
        public int? QuantParcelas { get; set; }

        [Required(ErrorMessage = "O funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public List<ItensVendaDTO> ItensVendas { get; set; }
    }

    public class ItensVendaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public double Valor { get; set; }

        [Required(ErrorMessage = "O produto é obrigatório")]
        public int ProdutoId { get; set; }

        public int VendaId { get; set; }
    }
}