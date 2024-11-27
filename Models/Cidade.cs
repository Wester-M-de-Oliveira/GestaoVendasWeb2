using System.Collections.Generic;

public class Cidade
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("nome")]
    public string Nome { get; set; }
    [ForeignKey("estado_id")]
    public int IdEstado { get; set; }
    [Column("estado")]
    public Estado Estado { get; set; }
}