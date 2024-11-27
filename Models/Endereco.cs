using System.Collections.Generic;

public class Endereco
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("rua")]
    public string Rua { get; set; }
    [Column("numero")]
    public int? Numero { get; set; }
    [Column("bairro")]
    public string Bairro { get; set; }
    [ForeignKey("cidade_id")]
    public int IdCidade { get; set; }
    [Column("cidade")]
    public Cidade Cidade { get; set; }
}