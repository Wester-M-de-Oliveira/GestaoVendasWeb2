using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestaoVendasWeb2.Models;
[Table("estado")]
public class Estado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("nome")]
    public string Nome { get; set; }

    [Column("sigla")]
    public string Sigla { get; set; }

    [JsonIgnore]
    public ICollection<Cidade> Cidades { get; set; }
}
