using System.Collections.Generic;

public class Estado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("nome")]
    public string Nome { get; set; }
    [Column("sigla")]
    public string Sigla { get; set; }
}