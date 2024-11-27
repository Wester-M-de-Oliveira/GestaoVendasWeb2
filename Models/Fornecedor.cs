using System.Collections.Generic;

public class Fornecedor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("razao_social")]
    public string RazaoSocial { get; set; }
    [Column("nome_fantasia")]
    public string NomeFantasia { get; set; }
    [ForeignKey("endereco_id")]
    public int IdEndereco { get; set; }
    [Column("endereco")]
    public Endereco Endereco { get; set; }
}