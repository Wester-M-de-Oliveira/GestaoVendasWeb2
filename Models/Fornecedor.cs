using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestaoVendasWeb2.Models 
{
    [Table("fornecedor")]
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
        [Column("endereco_id")]
        public int EnderecoId { get; set; }

        public Endereco Endereco { get; set; }

        [JsonIgnore]
        public ICollection<Compra> Compras { get; set; }
    }
}