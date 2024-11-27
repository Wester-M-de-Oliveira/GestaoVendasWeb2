﻿using System.Collections.Generic;

public class ItemPedido
{
    public int IdItemPedido { get; set; }
    public int IdPedido { get; set; }
    public Pedido Pedido { get; set; }
    public int IdProduto { get; set; }
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}