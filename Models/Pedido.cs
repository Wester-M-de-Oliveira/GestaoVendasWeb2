using System.Collections.Generic;

public class Pedido
{
    public int IdPedido { get; set; }
    public DateTime Data { get; set; }
    public decimal Total { get; set; }
    public Cliente Cliente { get; set; }
}
