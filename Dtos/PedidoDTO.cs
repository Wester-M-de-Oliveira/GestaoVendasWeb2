using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class PedidoDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total deve ser positivo")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public Cliente Cliente { get; set; }
    }
}
