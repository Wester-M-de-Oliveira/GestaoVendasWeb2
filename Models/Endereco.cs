using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestaoVendasWeb2.Models;
[Table("endereco")]
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
    [Column("cidade_id")]
    public int CidadeId { get; set; }
    public Cidade Cidade { get; set; }

    [JsonIgnore]
    public ICollection<Cliente> Clientes { get; set; }

    [JsonIgnore]
    public ICollection<Funcionario> Funcionarios { get; set; }

    [JsonIgnore]
    public ICollection<Fornecedor> Fornecedores { get; set; }

}
