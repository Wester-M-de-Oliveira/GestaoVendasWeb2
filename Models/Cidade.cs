using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GestaoVendasWeb2.Models;
[Table("cidade")]
public class Cidade
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("nome", TypeName = "varchar(200)")]
    public string Nome { get; set; }

    [ForeignKey("estado_id")]
    [Column("estado_id")]
    public int EstadoId { get; set; }

    public Estado Estado { get; set; }

    [JsonIgnore]
    public ICollection<Endereco> Enderecos { get; set; }

}