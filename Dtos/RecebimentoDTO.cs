using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class RecebimentoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [Display(Name = "Data do Recebimento")]
        [DataType(DataType.DateTime)]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Display(Name = "Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O caixa é obrigatório")]
        [Display(Name = "Caixa")]
        public int CaixaId { get; set; }

        [Required(ErrorMessage = "A venda é obrigatória")]
        [Display(Name = "Venda")]
        public int VendaId { get; set; }

        [Required(ErrorMessage = "O funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        public CaixaDTO Caixa { get; set; }
        public VendaDTO Venda { get; set; }
        public FuncionarioDTO Funcionario { get; set; }
    }
}