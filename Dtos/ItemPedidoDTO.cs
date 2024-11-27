using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
public class ItemPedidoDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    public Pedido Pedido { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public Produto Produto { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser ao menos 1")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser positivo")]
    public decimal PrecoUnitario { get; set; }
}
